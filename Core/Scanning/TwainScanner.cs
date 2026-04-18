using DocumentArchiever.Services;
using NTwain;
using NTwain.Data;
using NTwain.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DocumentArchiever.Core.Scanning
{
    public class TwainScanner : IScanner
    {
        private TwainAppSession _twainSession;
        private TWIdentityWrapper _currentSource;
        private bool _isScanning;
        private int _pageCounter;
        private List<string> _scannedImages;
        private ScanOptions _currentOptions;
        private readonly ILogger _logger;
        private TaskCompletionSource<bool> _scanCompletionSource;
        private readonly ImageCodecInfo _jpegEncoder;
        private readonly EncoderParameters _jpegParameters;
        private readonly int _jpegQuality = 75;

        public event EventHandler<ImageScannedEventArgs> ImageScanned;
        public event EventHandler<ScanProgressEventArgs> ScanProgress;
        public event EventHandler<ScanCompletedEventArgs> ScanCompleted;
        public event EventHandler<ScanErrorEventArgs> ScanError;

        public bool IsBusy => _isScanning;
        public bool IsConnected => _twainSession != null && _twainSession.State >= STATE.S4;
        public string Name => "TWAIN Scanner";

        public TwainScanner(ILogger logger)
        {
            _logger = logger;
            _scannedImages = new List<string>();

            // Setup JPEG encoder for saving images
            _jpegParameters = new EncoderParameters(1);
            _jpegParameters.Param[0] = new EncoderParameter(Encoder.Quality, (long)_jpegQuality);
            _jpegEncoder = ImageCodecInfo.GetImageEncoders().First(enc => enc.FormatID == ImageFormat.Jpeg.Guid);
        }

        public Task<List<ScannerInfo>> GetAvailableScannersAsync()
        {
            var scanners = new List<ScannerInfo>();

            try
            {
                // Create a temporary session to enumerate scanners
                using (var tempSession = new TwainAppSession(appThreadContext: SynchronizationContext.Current))
                {
                    tempSession.OpenDsm();

                    var dataSources = tempSession.GetSources();
                    for (int i = 0; i < dataSources.Count; i++)
                    {
                        var ds = dataSources[i];
                        scanners.Add(new ScannerInfo
                        {
                            Id = ds.ProductName, // Use ProductName as identifier
                            Name = ds.ProductName,
                            Type = ScannerType.Twain,
                            SupportsDuplex = true,
                            SupportsFeeder = true,
                            NativeDevice = ds
                        });
                    }

                    tempSession.CloseDsm();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enumerating TWAIN scanners");
            }

            return Task.FromResult(scanners);
        }

        public Task<bool> ConnectAsync(ScannerInfo scanner, CancellationToken cancellationToken = default)
        {
            try
            {
                DisconnectAsync().Wait();

                // Create the TwainAppSession with current synchronization context
                _twainSession = new TwainAppSession(appThreadContext: SynchronizationContext.Current);

                // Attach event handlers
                _twainSession.StateChanged += OnStateChanged;
                _twainSession.SourceDisabled += OnSourceDisabled;
                _twainSession.TransferReady += OnTransferReady;
                _twainSession.Transferred += OnTransferred;
                _twainSession.TransferError += OnTransferError;

                // Open the DSM (Data Source Manager)
                var result = _twainSession.OpenDsm();
                if (result.RC != TWRC.SUCCESS)
                {
                    _logger.LogError($"Failed to open DSM: {result}");
                    return Task.FromResult(false);
                }

                // Get the selected source
                var dataSources = _twainSession.GetSources();
                var selectedSource = dataSources.FirstOrDefault(ds => ds.ProductName == scanner.Name);

                if (selectedSource == null)
                {
                    _logger.LogError($"Scanner {scanner.Name} not found");
                    return Task.FromResult(false);
                }

                // Open the source
                result = _twainSession.OpenSource(selectedSource);
                if (result.RC != TWRC.SUCCESS)
                {
                    _logger.LogError($"Failed to open source: {result}");
                    return Task.FromResult(false);
                }

                _currentSource = selectedSource;

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect to TWAIN scanner");
                ScanError?.Invoke(this, new ScanErrorEventArgs { Exception = ex, Message = ex.Message, IsFatal = true });
                return Task.FromResult(false);
            }
        }

        public Task DisconnectAsync()
        {
            try
            {
                if (_twainSession != null)
                {
                    // Close source if open
                    if (_twainSession.State >= STATE.S4)
                    {
                        _twainSession.CloseSource();
                    }

                    // Close DSM
                    if (_twainSession.State >= STATE.S3)
                    {
                        _twainSession.CloseDsm();
                    }

                    _twainSession.Dispose();
                    _twainSession = null;
                }

                _currentSource = null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disconnecting TWAIN scanner");
            }

            return Task.CompletedTask;
        }
        /*
        public Task StartScanningAsync(ScanOptions options, CancellationToken cancellationToken = default)
        {
            if (_twainSession == null || _twainSession.State < STATE.S4)
            {
                throw new InvalidOperationException("Scanner not connected");
            }

            _isScanning = true;
            _currentOptions = options;
            _pageCounter = 0;
            _scannedImages.Clear();
            _scanCompletionSource = new TaskCompletionSource<bool>();

            // Configure scanner settings
            ConfigureScanner(options);

            // Enable the source to start scanning
            // Use NoUI for automatic scanning without dialog
            var enableResult = _twainSession.EnableSource(SourceEnableOption.NoUI);

            if (!enableResult.IsSuccess)
            {
                _isScanning = false;
                throw new InvalidOperationException($"Failed to enable source: {enableResult}");
            }

            return Task.CompletedTask;
        }*/
        public async Task StartScanningAsync(ScanOptions options, CancellationToken cancellationToken = default)
        {
            if (_twainSession == null || _twainSession.State < STATE.S4)
            {
                throw new InvalidOperationException("Scanner not connected");
            }

            _isScanning = true;
            _currentOptions = options;
            _pageCounter = 0;
            _scannedImages.Clear();
            _scanCompletionSource = new TaskCompletionSource<bool>();

            // Configure scanner settings
            ConfigureScanner(options);

            // Enable the source to start scanning
            var enableResult = _twainSession.EnableSource(SourceEnableOption.NoUI);

            if (!enableResult.IsSuccess)
            {
                _isScanning = false;
                throw new InvalidOperationException($"Failed to enable source: {enableResult}");
            }

            // Wait for scanning to complete
            await _scanCompletionSource.Task;
        }
        public Task StopScanningAsync()
        {
            _isScanning = false;

            if (_twainSession != null && _twainSession.State >= STATE.S4)
            {
                _twainSession.CloseSource();
            }

            _scanCompletionSource?.TrySetResult(true);

            return Task.CompletedTask;
        }

        private void ConfigureScanner(ScanOptions options)
        {
            try
            {
                // Enable document feeder
                var feederResult = _twainSession.SetCap(CAP.CAP_FEEDERENABLED, TW_BOOL.True);
                if (!feederResult.IsSuccess)
                {
                    _logger.LogWarning($"Failed to enable feeder: {feederResult}");
                }

                // Enable duplex if requested
                if (options.DuplexEnabled)
                {
                    var duplexResult = _twainSession.SetCap(CAP.CAP_DUPLEXENABLED, TW_BOOL.True);
                    if (!duplexResult.IsSuccess)
                    {
                        _logger.LogWarning($"Failed to enable duplex: {duplexResult}");
                    }
                }

                // Set color mode
                //  a_twimageinfo.PixelType = short.Parse(CvtCapValueFromEnum(CAP.ICAP_PIXELTYPE, asz[14]));
               //Tw_P
                //TW_PIXELTYPE pixelType = options.ColorMode == ColorMode.Color ? TW_PIXELTYPE.RGB :
                //                         options.ColorMode == ColorMode.Grayscale ? TW_PIXELTYPE.GRAY :
                //                         TW_PIXELTYPE.BW;

                NTwain.Data.TWPT pixelType = options.ColorMode == ColorMode.Color ? TWPT.RGB :
                                         options.ColorMode == ColorMode.Grayscale ? TWPT.GRAY :
                                         TWPT.BW;

                var pixelResult = _twainSession.SetCap(CAP.ICAP_PIXELTYPE, pixelType);
                if (!pixelResult.IsSuccess)
                {
                    _logger.LogWarning($"Failed to set pixel type: {pixelResult}");
                }

                // Set resolution
                var xresResult = _twainSession.SetCap(CAP.ICAP_XRESOLUTION, (float)options.Dpi);
                if (!xresResult.IsSuccess)
                {
                    _logger.LogWarning($"Failed to set X resolution: {xresResult}");
                }

                var yresResult = _twainSession.SetCap(CAP.ICAP_YRESOLUTION, (float)options.Dpi);
                if (!yresResult.IsSuccess)
                {
                    _logger.LogWarning($"Failed to set Y resolution: {yresResult}");
                }

                // Enable auto feed
                var autoFeedResult = _twainSession.SetCap(CAP.CAP_AUTOFEED, TW_BOOL.True);
                if (!autoFeedResult.IsSuccess)
                {
                    _logger.LogWarning($"Failed to set auto feed: {autoFeedResult}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Error configuring TWAIN scanner: {ex.Message}");
            }
        }

        private void OnTransferred(object sender, TransferredEventArgs e)
        {
            _logger.LogInfo($"[Thread {Environment.CurrentManagedThreadId}] Data transferred");

            try
            {

                // Process the image data - either take ownership or use directly
                using (var data = e.TakeDataOwnership())
                {
                    if (data != null)
                    {
                        string tempPath = Path.Combine(Path.GetTempPath(), $"scan_{DateTime.Now.Ticks}_{_pageCounter}.jpg");

                        // Save the image using System.Drawing
                        using (var img = Image.FromStream(data.AsStream()))
                        {
                            // Reduce resolution if needed
                            using (var reducedImage = ReduceResolution((Bitmap)img, _currentOptions.Dpi))
                            {
                                reducedImage.Save(tempPath, _jpegEncoder, _jpegParameters);
                            }
                        }

                        _scannedImages.Add(tempPath);

                        bool isFrontSide = _pageCounter % 2 == 0;

                        ImageScanned?.Invoke(this, new ImageScannedEventArgs
                        {
                            ImagePath = tempPath,
                            PageNumber = _pageCounter + 1,
                            IsFrontSide = isFrontSide,
                            ScannedAt = DateTime.Now
                        });

                        _pageCounter++;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing TWAIN data transfer");
                ScanError?.Invoke(this, new ScanErrorEventArgs { Exception = ex, Message = ex.Message });
            }
        }

        private Bitmap ReduceResolution(Bitmap original, int targetDpi)
        {
            if (original.HorizontalResolution <= targetDpi)
                return original;

            int newWidth = (int)(original.Width * (targetDpi / original.HorizontalResolution));
            int newHeight = (int)(original.Height * (targetDpi / original.VerticalResolution));

            Bitmap resized = new Bitmap(newWidth, newHeight);
            resized.SetResolution(targetDpi, targetDpi);

            using (Graphics g = Graphics.FromImage(resized))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(original, 0, 0, newWidth, newHeight);
            }

            return resized;
        }

        private void OnTransferReady(object sender, TransferReadyEventArgs e)
        {
            Debug.WriteLine($"[Thread {Environment.CurrentManagedThreadId}] Transfer ready");

            ScanProgress?.Invoke(this, new ScanProgressEventArgs
            {
                PagesScanned = _pageCounter,
                Message = "Ready to transfer image..."
            });
        }

        private void OnTransferError(object sender, TransferErrorEventArgs e)
        {
            Debug.WriteLine($"[Thread {Environment.CurrentManagedThreadId}] Transfer error: {e.Exception?.Message ?? e.Code.ToString()}");

            _logger.LogError(e.Exception, "TWAIN transfer error");
            ScanError?.Invoke(this, new ScanErrorEventArgs
            {
                Exception = e.Exception,
                Message = e.Exception?.Message ?? e.Code.ToString()
            });
        }

        private void OnSourceDisabled(object sender, TWIdentityWrapper e)
        {
            Debug.WriteLine($"Source disabled");

            _isScanning = false;
            _scanCompletionSource?.TrySetResult(true);

            ScanCompleted?.Invoke(this, new ScanCompletedEventArgs
            {
                TotalPages = _pageCounter,
                ImagePaths = new List<string>(_scannedImages),
                Duration = TimeSpan.Zero
            });
        }

        private void OnStateChanged(object sender, STATE state)
        {
            Debug.WriteLine($"State changed to: {state}");
            _logger.LogInfo($"TWAIN State: {state}");
        }

        public void Dispose()
        {
            DisconnectAsync().Wait(5000);
            _jpegParameters?.Dispose();
        }
    }
}