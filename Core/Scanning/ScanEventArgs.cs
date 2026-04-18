namespace DocumentArchiever.Core.Scanning
{
    public class ImageScannedEventArgs : EventArgs
    {
        public string ImagePath { get; set; }
        public int PageNumber { get; set; }
        public bool IsFrontSide { get; set; }
        public DateTime ScannedAt { get; set; }
    }

    public class ScanProgressEventArgs : EventArgs
    {
        public int PagesScanned { get; set; }
        public string Message { get; set; }
    }

    public class ScanCompletedEventArgs : EventArgs
    {
        public int TotalPages { get; set; }
        public List<string> ImagePaths { get; set; }
        public TimeSpan Duration { get; set; }
    }

    public class ScanErrorEventArgs : EventArgs
    {
        public Exception Exception { get; set; }
        public string Message { get; set; }
        public bool IsFatal { get; set; }
    }
}