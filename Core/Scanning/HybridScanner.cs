using DocumentArchiever.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DocumentArchiever.Core.Scanning
{
    public class HybridScanner : IScanner
    {
        private IScanner _activeScanner;
        private readonly TwainScanner _twainScanner;
        private readonly WiaScanner _wiaScanner;
        private readonly ILogger _logger;

        public event EventHandler<ImageScannedEventArgs> ImageScanned
        {
            add { if (_activeScanner != null) _activeScanner.ImageScanned += value; }
            remove { if (_activeScanner != null) _activeScanner.ImageScanned -= value; }
        }

        public event EventHandler<ScanProgressEventArgs> ScanProgress
        {
            add { if (_activeScanner != null) _activeScanner.ScanProgress += value; }
            remove { if (_activeScanner != null) _activeScanner.ScanProgress -= value; }
        }

        public event EventHandler<ScanCompletedEventArgs> ScanCompleted
        {
            add { if (_activeScanner != null) _activeScanner.ScanCompleted += value; }
            remove { if (_activeScanner != null) _activeScanner.ScanCompleted -= value; }
        }

        public event EventHandler<ScanErrorEventArgs> ScanError
        {
            add { if (_activeScanner != null) _activeScanner.ScanError += value; }
            remove { if (_activeScanner != null) _activeScanner.ScanError -= value; }
        }

        public bool IsBusy => _activeScanner?.IsBusy ?? false;
        public bool IsConnected => _activeScanner?.IsConnected ?? false;
        public string Name => _activeScanner?.Name ?? "Hybrid Scanner";

        public HybridScanner(ILogger logger)
        {
            _logger = logger;
            _twainScanner = new TwainScanner(logger);
            _wiaScanner = new WiaScanner(logger);
        }

        public async Task<List<ScannerInfo>> GetAvailableScannersAsync()
        {
            var allScanners = new List<ScannerInfo>();

            var twainScanners = await _twainScanner.GetAvailableScannersAsync();
            var wiaScanners = await _wiaScanner.GetAvailableScannersAsync();

            allScanners.AddRange(twainScanners);
            allScanners.AddRange(wiaScanners);

            return allScanners;
        }

        public async Task<bool> ConnectAsync(ScannerInfo scanner, CancellationToken cancellationToken = default)
        {
            bool result = false;

            switch (scanner.Type)
            {
                case ScannerType.Twain:
                    result = await _twainScanner.ConnectAsync(scanner, cancellationToken);
                    if (result) _activeScanner = _twainScanner;
                    break;
                case ScannerType.Wia:
                    result = await _wiaScanner.ConnectAsync(scanner, cancellationToken);
                    if (result) _activeScanner = _wiaScanner;
                    break;
            }

            return result;
        }

        public async Task DisconnectAsync()
        {
            if (_activeScanner != null)
            {
                await _activeScanner.DisconnectAsync();
                _activeScanner = null;
            }
        }

        public async Task StartScanningAsync(ScanOptions options, CancellationToken cancellationToken = default)
        {
            if (_activeScanner == null)
            {
                throw new InvalidOperationException("No scanner connected");
            }

            await _activeScanner.StartScanningAsync(options, cancellationToken);
        }

        public async Task StopScanningAsync()
        {
            if (_activeScanner != null)
            {
                await _activeScanner.StopScanningAsync();
            }
        }

        public void Dispose()
        {
            _twainScanner?.Dispose();
            _wiaScanner?.Dispose();
        }
    }
}