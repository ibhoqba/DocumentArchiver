using DocumentArchiever.Services;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tesseract;

namespace DocumentArchiever.Core.OCR
{
    public class TesseractOcrEngine : IOcrEngine
    {
        private static readonly string TessDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata");
        private readonly ILogger _logger;
        private TesseractEngine _arabicEngine;
        private TesseractEngine _englishEngine;
        private bool _disposed;
        public static int ExpressNumber =599;
         public  TesseractOcrEngine(ILogger logger)
        {
            _logger = logger;
        }

        public static void Initialize()
        {
            if (!Directory.Exists(TessDataPath))
            {
                Directory.CreateDirectory(TessDataPath);
                MessageBox.Show($"Please add Tesseract language files to: {TessDataPath}", "OCR Initialization",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            Environment.SetEnvironmentVariable("OMP_THREAD_LIMIT", "2");
            Environment.SetEnvironmentVariable("TESSDATA_PREFIX", TessDataPath);
        }

        private void EnsureEnginesInitialized()
        {
            if (_arabicEngine == null)
            {
                _arabicEngine = new TesseractEngine(TessDataPath, "ara", EngineMode.Default);
                _arabicEngine.SetVariable("preserve_interword_spaces", "1");
            }

            if (_englishEngine == null)
            {
                _englishEngine = new TesseractEngine(TessDataPath, "eng", EngineMode.Default);
            }
        }

        public async Task<string> ExtractTextAsync(string imagePath)
        {
            return await Task.Run(() =>
            {
                EnsureEnginesInitialized();

                try
                {
                    using (var img = Pix.LoadFromFile(imagePath))
                    using (var page = _arabicEngine.Process(img))
                    {
                        return page.GetText().Trim();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"OCR failed for {imagePath}");
                    return string.Empty;
                }
            });
        }
       
        public async Task<string> ExtractExpressNumberAsync(string imagePath)
        {
            return await Task.Run(() =>
            {
                EnsureEnginesInitialized();

                try
                {
                    using (var img = Pix.LoadFromFile(imagePath))
                    using (var page = _englishEngine.Process(img))
                    {
                        string text = page.GetText().Trim();
                        var match = Regex.Match(text, @$"{ExpressNumber}\d{6}");
                        return match.Success ? match.Value : string.Empty;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Express number extraction failed for {imagePath}");
                    return string.Empty;
                }
            });
        }

        public async Task<string> ExtractExpressNumberFromPdfAsync(string pdfPath)
        {
            return await Task.Run(async () =>
            {
                string result = string.Empty;

                try
                {
                    using (var reader = new iTextSharp.text.pdf.PdfReader(pdfPath))
                    {
                        for (int i = 1; i <= Math.Min(reader.NumberOfPages, 3); i++)
                        {
                            var text = iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(reader, i);
                            var match = Regex.Match(text, @$"{ExpressNumber}\d{6}");
                            if (match.Success)
                            {
                                result = match.Value;
                                break;
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(result))
                    {
                        result = await ExtractWithGhostscriptAsync(pdfPath);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"PDF express number extraction failed for {pdfPath}");
                }

                return result;
            });
        }

        private async Task<string> ExtractWithGhostscriptAsync(string pdfPath)
        {
            return await Task.Run(() =>
            {
                // Ghostscript extraction logic
                return string.Empty;
            });
        }

        public async Task<DocumentType> IdentifyDocumentTypeAsync(string imagePath)
        {
            return await Task.Run(() =>
            {
                EnsureEnginesInitialized();

                try
                {
                    using (var img = Pix.LoadFromFile(imagePath))
                    using (var page = _arabicEngine.Process(img))
                    {
                        string text = page.GetText().ToLower();

                        if (Regex.IsMatch(text, @$"{ExpressNumber}\d{6}") || text.Contains("ÕÊ«·…"))
                            return DocumentType.TransferVoucher;

                        if (text.Contains("»Ì‰ «·›—Ê⁄"))
                            return DocumentType.InterBranchCash;

                        if (text.Contains("⁄„·«¡ ‰ﬁœ"))
                            return DocumentType.CashVoucher;

                        return DocumentType.Unknown;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Document type identification failed for {imagePath}");
                    return DocumentType.Unknown;
                }
            });
        }
      
        public async Task<string> ExtractVoucherNumberAsync(string imagePath)
        {
            return await Task.Run(() =>
            {
                EnsureEnginesInitialized();

                try
                {
                    using (var image = new Bitmap(imagePath))
                    {
                        int cropHeight = image.Height / 4;
                        int startY = image.Height / 2 - cropHeight / 2;

                        using (var cropped = image.Clone(new Rectangle(0, startY, image.Width, cropHeight), image.PixelFormat))
                        using (var ms = new MemoryStream())
                        {
                            cropped.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                            ms.Position = 0;

                            using (var pix = Pix.LoadFromMemory(ms.ToArray()))
                            using (var deskewed = pix.Deskew())
                            using (var page = _englishEngine.Process(deskewed))
                            {
                                _englishEngine.SetVariable("tessedit_char_whitelist", "0123456789");
                                string numbers = page.GetText().Trim();
                                _englishEngine.SetVariable("tessedit_char_whitelist", "");

                                var match = Regex.Match(numbers, @"\d{3,8}");
                                return match.Success ? match.Value : string.Empty;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Voucher number extraction failed for {imagePath}");
                    return string.Empty;
                }
            });
        }

        public async Task<string> ExtractNumberFromRegionAsync(string imagePath, OcrRegion region)
        {
            return await Task.Run(() =>
            {
                EnsureEnginesInitialized();

                try
                {
                    using (var image = new Bitmap(imagePath))
                    using (var cropped = image.Clone(new Rectangle(region.X, region.Y, region.Width, region.Height), image.PixelFormat))
                    using (var ms = new MemoryStream())
                    {
                        cropped.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        ms.Position = 0;

                        using (var pix = Pix.LoadFromMemory(ms.ToArray()))
                        using (var page = _englishEngine.Process(pix))
                        {
                            _englishEngine.SetVariable("tessedit_char_whitelist", "0123456789");
                            string result = page.GetText().Trim();
                            _englishEngine.SetVariable("tessedit_char_whitelist", "");
                            return result;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Region OCR failed for {imagePath}");
                    return string.Empty;
                }
            });
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _arabicEngine?.Dispose();
                _englishEngine?.Dispose();
                _disposed = true;
            }
        }
    }
}