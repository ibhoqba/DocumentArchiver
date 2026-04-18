using DocumentArchiever.Services;
using System.Runtime.InteropServices;
using WIA;

namespace DocumentArchiever.Core.Scanning
{
    public class WiaScanner : IScanner
    {
        private Device _wiaDevice;
        private DeviceInfo _currentDeviceInfo;
        private bool _isScanning;
        private int _pageCounter;
        private List<string> _scannedImages;
        private readonly ILogger _logger;

        public event EventHandler<ImageScannedEventArgs> ImageScanned;
        public event EventHandler<ScanProgressEventArgs> ScanProgress;
        public event EventHandler<ScanCompletedEventArgs> ScanCompleted;
        public event EventHandler<ScanErrorEventArgs> ScanError;

        public bool IsBusy => _isScanning;
        public bool IsConnected => _wiaDevice != null;
        public string Name => "WIA Scanner";

        public WiaScanner(ILogger logger)
        {
            _logger = logger;
            _scannedImages = new List<string>();
        }

        public async Task<List<ScannerInfo>> GetAvailableScannersAsync()
        {
            return await Task.Run(() =>
            {
                var scanners = new List<ScannerInfo>();

                try
                {
                    DeviceManager deviceManager = new DeviceManager();

                    foreach (DeviceInfo deviceInfo in deviceManager.DeviceInfos)
                    {
                        if (deviceInfo.Type == WiaDeviceType.ScannerDeviceType)
                        {
                            scanners.Add(new ScannerInfo
                            {
                                Id = deviceInfo.DeviceID,
                                Name = deviceInfo.Properties["Name"].get_Value().ToString(),
                                Type = ScannerType.Wia,
                                SupportsDuplex = true,
                                SupportsFeeder = true,
                                NativeDevice = deviceInfo
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error enumerating WIA scanners");
                }

                return scanners;
            });
        }

        public async Task<bool> ConnectAsync(ScannerInfo scanner, CancellationToken cancellationToken = default)
        {
            return await Task.Run(() =>
            {
                try
                {
                    DisconnectAsync().Wait();

                    _currentDeviceInfo = scanner.NativeDevice as DeviceInfo;
                    if (_currentDeviceInfo == null)
                    {
                        return false;
                    }

                    _wiaDevice = _currentDeviceInfo.Connect();
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to connect to WIA scanner");
                    ScanError?.Invoke(this, new ScanErrorEventArgs { Exception = ex, Message = ex.Message, IsFatal = true });
                    return false;
                }
            }, cancellationToken);
        }

        public async Task DisconnectAsync()
        {
            await Task.Run(() =>
            {
                try
                {
                    if (_wiaDevice != null)
                    {
                        Marshal.ReleaseComObject(_wiaDevice);
                        _wiaDevice = null;
                    }
                    _currentDeviceInfo = null;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error disconnecting WIA scanner");
                }
            });
        }

        public async Task StartScanningAsync(ScanOptions options, CancellationToken cancellationToken = default)
        {
            await Task.Run(() =>
            {
                if (_wiaDevice == null)
                {
                    throw new InvalidOperationException("Scanner not connected");
                }

                _isScanning = true;
                _pageCounter = 0;
                _scannedImages.Clear();

                IItem scannerItem = _wiaDevice.Items[1];
                ConfigureScanner(scannerItem, options);

                ScanWiaPages(scannerItem, options, cancellationToken);
            }, cancellationToken);
        }

        private void ConfigureScanner(IItem scannerItem, ScanOptions options)
        {
            try
            {
                int colorIntent = options.ColorMode == ColorMode.Color ? 2 :
                                 options.ColorMode == ColorMode.Grayscale ? 1 : 0;
                SetWiaProperty(scannerItem.Properties, "6146", colorIntent);

                SetWiaProperty(scannerItem.Properties, "6147", options.Dpi);
                SetWiaProperty(scannerItem.Properties, "6148", options.Dpi);

                if (options.DuplexEnabled)
                {
                    SetWiaProperty(scannerItem.Properties, "3088", 0x004);
                }
                else
                {
                    SetWiaProperty(scannerItem.Properties, "3088", 0x002);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Error configuring WIA scanner: {ex.Message}");
            }
        }

        private void SetWiaProperty(IProperties properties, string propertyName, object value)
        {
            try
            {
                Property prop = properties.get_Item(propertyName);
                if (prop != null)
                {
                    prop.set_Value(value);
                }
            }
            catch
            {
                // Property not supported
            }
        }

        private void ScanWiaPages(IItem scannerItem, ScanOptions options, CancellationToken cancellationToken)
        {
            bool hasMorePages = true;

            while (hasMorePages && _isScanning && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    ImageFile image = (ImageFile)scannerItem.Transfer("{B96B3CAF-0728-11D3-9D7B-0000F81EF32E}");
                    string tempPath = Path.Combine(Path.GetTempPath(), $"wia_scan_{DateTime.Now.Ticks}_{_pageCounter}.jpg");
                    image.SaveFile(tempPath);

                    _scannedImages.Add(tempPath);

                    ImageScanned?.Invoke(this, new ImageScannedEventArgs
                    {
                        ImagePath = tempPath,
                        PageNumber = _pageCounter + 1,
                        IsFrontSide = _pageCounter % 2 == 0,
                        ScannedAt = DateTime.Now
                    });

                    _pageCounter++;
                    Marshal.ReleaseComObject(image);

                    hasMorePages = HasMorePages(scannerItem);
                }
                catch (COMException comEx) when ((uint)comEx.ErrorCode == 0x80210003)
                {
                    hasMorePages = false;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during WIA scan");
                    ScanError?.Invoke(this, new ScanErrorEventArgs { Exception = ex, Message = ex.Message });
                    hasMorePages = false;
                }
            }

            _isScanning = false;
            ScanCompleted?.Invoke(this, new ScanCompletedEventArgs
            {
                TotalPages = _pageCounter,
                ImagePaths = new List<string>(_scannedImages),
                Duration = TimeSpan.Zero
            });
        }

        private bool HasMorePages(IItem scannerItem)
        {
            try
            {
                Property statusProp = scannerItem.Properties.get_Item("3088");
                if (statusProp != null)
                {
                    int status = (int)statusProp.get_Value();
                    return (status & 0x01) == 0;
                }
            }
            catch
            {
                // Property not supported
            }
            return true;
        }

        public Task StopScanningAsync()
        {
            _isScanning = false;
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            DisconnectAsync().Wait();
        }
    }
}