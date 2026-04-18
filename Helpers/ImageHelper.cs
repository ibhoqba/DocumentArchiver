using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace DocumentArchiever.Helpers
{
    public static class ImageHelper
    {
        public static bool IsMostlyWhite(Bitmap image, double threshold)
        {
            int sampleStep = Math.Max(1, Math.Min(image.Width, image.Height) / 20);
            int whitePixels = 0;
            int totalPixels = 0;

            for (int y = 0; y < image.Height; y += sampleStep)
            {
                for (int x = 0; x < image.Width; x += sampleStep)
                {
                    Color pixel = image.GetPixel(x, y);
                    if (pixel.R > 240 && pixel.G > 240 && pixel.B > 240)
                        whitePixels++;
                    totalPixels++;
                }
            }

            if (totalPixels == 0) return true;
            return (double)whitePixels / totalPixels >= threshold;
        }

        public static bool IsImageBlank(Bitmap image, double whiteRatioThreshold = 0.95, double brightnessThreshold = 220)
        {
            int whitePixels = 0;
            int totalPixels = 0;

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixel = image.GetPixel(x, y);
                    double luminance = 0.299 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B;
                    if (luminance >= brightnessThreshold)
                        whitePixels++;
                    totalPixels++;
                }
            }

            return totalPixels == 0 || (double)whitePixels / totalPixels >= whiteRatioThreshold;
        }

        public static Bitmap ReduceResolution(Bitmap original, int targetDpi)
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

        public static Bitmap CropImage(Bitmap image, Rectangle cropArea)
        {
            return image.Clone(cropArea, image.PixelFormat);
        }

        public static void SaveWithRetry(Bitmap image, string filePath, int maxRetries = 3)
        {
            int retries = 0;
            while (retries < maxRetries)
            {
                try
                {
                    string directory = Path.GetDirectoryName(filePath);
                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);

                    image.Save(filePath, ImageFormat.Jpeg);
                    return;
                }
                catch (IOException)
                {
                    retries++;
                    if (retries >= maxRetries) throw;
                    System.Threading.Thread.Sleep(1000);
                }
            }
        }
    }
}