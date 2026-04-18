namespace DocumentArchiever.Core.Scanning
{
    public interface IScanner : IDisposable
    {
        event EventHandler<ImageScannedEventArgs> ImageScanned;
        event EventHandler<ScanProgressEventArgs> ScanProgress;
        event EventHandler<ScanCompletedEventArgs> ScanCompleted;
        event EventHandler<ScanErrorEventArgs> ScanError;

        bool IsBusy { get; }
        bool IsConnected { get; }
        string Name { get; }

        Task<List<ScannerInfo>> GetAvailableScannersAsync();
        Task<bool> ConnectAsync(ScannerInfo scanner, CancellationToken cancellationToken = default);
        Task DisconnectAsync();
        Task StartScanningAsync(ScanOptions options, CancellationToken cancellationToken = default);
        Task StopScanningAsync();
    }
}