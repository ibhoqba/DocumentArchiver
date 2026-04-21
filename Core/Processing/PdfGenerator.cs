using DocumentArchiever.Services;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfDocument = PdfSharp.Pdf.PdfDocument;

namespace DocumentArchiever.Core.Processing
{
    public class PdfGenerator
    {

        public PdfGenerator(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private readonly ILogger _logger;
      
        public async Task GenerateAsync(string outputPath, string frontImagePath, string backImagePath)
        {
            await Task.Run(() =>
            {

                _logger.LogInfo($"GenerateAsync START: {outputPath}");
                _logger.LogInfo($"  Front image: {frontImagePath} (exists: {File.Exists(frontImagePath)})");
                _logger.LogInfo($"  Back image: {backImagePath} (exists: {File.Exists(backImagePath)})");
                using (var document = new PdfDocument())
                {
                    _logger.LogInfo("Adding Front page...");
                    AddPageToDocument(document, frontImagePath);

                    if (!string.IsNullOrEmpty(backImagePath) && File.Exists(backImagePath))
                    {
                        _logger.LogInfo("Adding back page...");
                        AddPageToDocument(document, backImagePath);
                    }
                    _logger.LogInfo($"Saving PDF to: {outputPath}");
                    document.Save(outputPath);
                    _logger.LogInfo("PDF saved successfully");

                }
            });
        }
        public async Task GenerateSinglePageAsync(string outputPath, string imagePath)
        {
            await Task.Run(() =>
            {
                using (var document = new PdfDocument())
                {
                    AddPageToDocument(document, imagePath);
                    document.Save(outputPath);
                }
            });
        }

        private void AddPageToDocument(PdfDocument document, string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath) || !File.Exists(imagePath))
                return;

            PdfPage page = document.AddPage();
            page.Size = PageSize.A4;

            using (XGraphics gfx = XGraphics.FromPdfPage(page))
            using (XImage image = XImage.FromFile(imagePath))
            {
                double pageWidth = page.Width;
                double pageHeight = page.Height;
                double imageWidth = image.PixelWidth * 72.0 / 300.0;
                double imageHeight = image.PixelHeight * 72.0 / 300.0;

                double scale = Math.Min(pageWidth / imageWidth, pageHeight / imageHeight);
                double scaledWidth = imageWidth * scale;
                double scaledHeight = imageHeight * scale;
                double offsetX = (pageWidth - scaledWidth) / 2;
                double offsetY = (pageHeight - scaledHeight) / 2;

                gfx.DrawImage(image, offsetX, offsetY, scaledWidth, scaledHeight);
            }
        }
    }
}