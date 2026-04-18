using System.Linq;

namespace DocumentArchiever.Helpers
{
    public static class ValidationHelper
    {
        public static bool IsValidNumber(string input, int minLength = 1, int maxLength = 9)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            return input.All(char.IsDigit) && input.Length >= minLength && input.Length <= maxLength;
        }

        public static bool IsValidExpressNumber(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            return input.StartsWith($"{Core.OCR.TesseractOcrEngine.ExpressNumber}") && input.Length == 9 && input.All(char.IsDigit);
        }

        public static bool IsValidYear(int year)
        {
            return year >= 2000 && year <= DateTime.Now.Year + 1;
        }

        public static bool IsValidDpi(int dpi)
        {
            return dpi >= 100 && dpi <= 600;
        }
    }
}