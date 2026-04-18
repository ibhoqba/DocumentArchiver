namespace DocumentArchiever.Core.Scanning
{
    public class ScanOptions
    {
        public int Dpi { get; set; } = 150;
        public ColorMode ColorMode { get; set; } = ColorMode.Color;
        public bool DuplexEnabled { get; set; } = true;
        public bool UseFeeder { get; set; } = true;
        public bool ContinuousMode { get; set; } = false;

        public static ScanOptions Default => new ScanOptions();
    }

    public enum ColorMode
    {
        BlackWhite,
        Grayscale,
        Color
    }
}