using DocumentArchiever.Services;
using System.Drawing.Imaging;

namespace DocumentArchiever.Core.Processing
{
    public class ImageProcessor
    {
        private readonly ILogger _logger;

        public ImageProcessor(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<bool> SaveImageWithRetryAsync(Image image, string filePath, int maxRetries = 5)
        {
            return await Task.Run(() =>
            {
                int retryCount = 0;

                while (retryCount < maxRetries)
                {
                    try
                    {
                        string directory = Path.GetDirectoryName(filePath);
                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }

                        image.Save(filePath, ImageFormat.Jpeg);
                        return true;
                    }
                    catch (IOException ex) when (ex.Message.Contains("network path") || ex.Message.Contains("network name"))
                    {
                        retryCount++;
                        if (retryCount >= maxRetries)
                        {
                            _logger.LogError($"Failed to save image after {maxRetries} retries: {filePath}");
                            return false;
                        }
                        Thread.Sleep(3000);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Failed to save image: {filePath}");
                        return false;
                    }
                }

                return false;
            });
        }

        public bool IsImageBlank(string imagePath, double whiteThreshold = 0.95)
        {
            try
            {
                using (var image = (Bitmap)Image.FromFile(imagePath))
                {
                    int whitePixels = 0;
                    int totalPixels = 0;

                    for (int y = 0; y < image.Height; y++)
                    {
                        for (int x = 0; x < image.Width; x++)
                        {
                            Color pixel = image.GetPixel(x, y);
                            double luminance = 0.299 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B;
                            if (luminance >= 220)
                                whitePixels++;
                            totalPixels++;
                        }
                    }

                    return (double)whitePixels / totalPixels >= whiteThreshold;
                }
            }
            catch
            {
                return true;
            }
        }

        public Bitmap ReduceResolution(Bitmap original, int targetDpi)
        {
            if (original.HorizontalResolution <= targetDpi)
                return original;

            int newWidth = (int)(original.Width * (targetDpi / original.HorizontalResolution));
            int newHeight = (int)(original.Height * (targetDpi / original.VerticalResolution));

            var resized = new Bitmap(newWidth, newHeight);
            resized.SetResolution(targetDpi, targetDpi);

            using (Graphics g = Graphics.FromImage(resized))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(original, 0, 0, newWidth, newHeight);
            }

            return resized;
        }

        public async Task<bool> DeleteFileWithRetryAsync(string path, int maxRetries = 3)
        {
            return await Task.Run(() =>
            {
                if (string.IsNullOrEmpty(path) || !File.Exists(path))
                    return true;

                for (int i = 0; i < maxRetries; i++)
                {
                    try
                    {
                        File.Delete(path);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning($"Failed to delete {path} (attempt {i + 1}): {ex.Message}");
                        Thread.Sleep(100);
                    }
                }

                return false;
            });
        }
    }
}