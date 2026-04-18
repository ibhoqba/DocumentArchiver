using System.Threading.Tasks;

namespace DocumentArchiever.Core.OCR
{
    public interface IOcrEngine : IDisposable
    {
        Task<string> ExtractTextAsync(string imagePath);
        Task<string> ExtractExpressNumberAsync(string imagePath);
        Task<string> ExtractExpressNumberFromPdfAsync(string pdfPath);
        Task<DocumentType> IdentifyDocumentTypeAsync(string imagePath);
        Task<string> ExtractVoucherNumberAsync(string imagePath);
        Task<string> ExtractNumberFromRegionAsync(string imagePath, OcrRegion region);
       
    }

    public enum DocumentType
    {
        Unknown = 0,
        TransferVoucher = 1,
        InterBranchCash = 2,
        CashVoucher = 3
        

    }
   

    public class OcrRegion
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public OcrRegion(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
}