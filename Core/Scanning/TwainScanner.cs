using DocumentArchiever.Services;
using NTwain;
using NTwain.Data;
using NTwain.Events;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace DocumentArchiever.Core.Scanning
{
    public class TwainScanner : IScanner
    {
        private readonly TwainStaThread _twainThread;
        private TwainAppSession _twainSession;
        private TWIdentityWrapper _currentSource;
        private bool _isScanning;
        private int _pageCounter;
        private List<string> _scannedImages;
        private ScanOptions _currentOptions;
        private readonly ILogger _logger;
        private TaskCompletionSource<bool> _sourceDisabledTcs;
        private TaskCompletionSource<bool> _transfersCompletedTcs;
        private readonly ImageCodecInfo _jpegEncoder;
        private readonly EncoderParameters _jpegParameters;
        private readonly int _jpegQuality = 75;
        private readonly object _transferSync = new();
        private Task _transferProcessingChain = Task.CompletedTask;
        private int _activeTransfers;
        private bool _sourceDisabledRaised;
        private bool _isDisposed;

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
            _twainThread = new TwainStaThread();
            _scannedImages = new List<string>();

            // Diagnostics: log instance identity

            //try
            //{
            //    _logger.LogInfo($"TwainScanner instance created. Hash: {this.GetHashCode()}");
            //}
            //catch { }

            // Setup JPEG encoder for saving images
            _jpegParameters = new EncoderParameters(1);
            _jpegParameters.Param[0] = new EncoderParameter(Encoder.Quality, (long)_jpegQuality);
            _jpegEncoder = ImageCodecInfo.GetImageEncoders().First(enc => enc.FormatID == ImageFormat.Jpeg.Guid);
        }
        public Task<List<ScannerInfo>> GetAvailableScannersAsync()
        {
            return _twainThread.InvokeAsync(() =>
            {
                var scanners = new List<ScannerInfo>();

                try
                {
                    using var tempSession = new TwainAppSession(appThreadContext: SynchronizationContext.Current);
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
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error enumerating TWAIN scanners");
                }

                return scanners;
            });
        }

        public async Task<bool> ConnectAsync(ScannerInfo scanner, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                return await _twainThread.InvokeAsync(() =>
                {
                    DisconnectInternal();

                    _twainSession = new TwainAppSession(appThreadContext: SynchronizationContext.Current);

                    _twainSession.StateChanged += OnStateChanged;
                    _twainSession.SourceDisabled += OnSourceDisabled;
                    _twainSession.TransferReady += OnTransferReady;
                    _twainSession.Transferred += OnTransferred;
                    _twainSession.TransferError += OnTransferError;

                    var result = _twainSession.OpenDsm();
                    if (result.RC != TWRC.SUCCESS)
                    {
                        _logger.LogError($"Failed to open DSM: {result}");
                        DisconnectInternal();
                        return false;
                    }

                    var dataSources = _twainSession.GetSources();
                    var selectedSource = dataSources.FirstOrDefault(ds => ds.ProductName == scanner.Name);

                    if (selectedSource == null)
                    {
                        _logger.LogError($"Scanner {scanner.Name} not found");
                        DisconnectInternal();
                        return false;
                    }

                    result = _twainSession.OpenSource(selectedSource);
                    if (result.RC != TWRC.SUCCESS)
                    {
                        _logger.LogError($"Failed to open source: {result}");
                        DisconnectInternal();
                        return false;
                    }

                    _currentSource = selectedSource;
                    return true;
                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect to TWAIN scanner");
                ScanError?.Invoke(this, new ScanErrorEventArgs { Exception = ex, Message = ex.Message, IsFatal = true });
                return false;
            }
        }

        public async Task DisconnectAsync()
        {
            try
            {
                await _twainThread.InvokeAsync(DisconnectInternal).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disconnecting TWAIN scanner");
            }
        }
        public async Task StartScanningAsync(ScanOptions options, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await _twainThread.InvokeAsync(() =>
            {
                if (_twainSession == null || _twainSession.State < STATE.S4)
                {
                    throw new InvalidOperationException("Scanner not connected");
                }

                _isScanning = true;
                _currentOptions = options;
                _pageCounter = 0;
                _scannedImages.Clear();
                _sourceDisabledRaised = false;
                _activeTransfers = 0;
                _sourceDisabledTcs = CreateCompletionSource();
                _transfersCompletedTcs = CreateCompletionSource(completed: true);

                ConfigureScanner(options);

                var enableResult = _twainSession.EnableSource(SourceEnableOption.NoUI);
                if (!enableResult.IsSuccess)
                {
                    _isScanning = false;
                    throw new InvalidOperationException($"Failed to enable source: {enableResult}");
                }
            }).ConfigureAwait(false);

            using var registration = cancellationToken.Register(() => _ = StopScanningAsync());

            await _sourceDisabledTcs.Task.ConfigureAwait(false);
            await _transfersCompletedTcs.Task.ConfigureAwait(false);
        }
        public async Task StopScanningAsync()
        {
            _isScanning = false;

            try
            {
                await _twainThread.InvokeAsync(() =>
                {
                    if (_twainSession != null && _twainSession.State >= STATE.S4)
                    {
                        _twainSession.CloseSource();
                    }
                }).ConfigureAwait(false);
            }
            finally
            {
                _sourceDisabledTcs?.TrySetResult(true);
                _transfersCompletedTcs?.TrySetResult(true);
            }
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
            try
            {
                byte[] imageBytes = null;
                using (var data = e.TakeDataOwnership())
                {
                    if (data != null)
                    {
                        using var stream = data.AsStream();
                        using var memory = new MemoryStream();
                        stream.CopyTo(memory);
                        imageBytes = memory.ToArray();
                    }
                }

                if (imageBytes == null || imageBytes.Length == 0)
                {
                    return;
                }

                BeginTransferProcessing();
                QueueTransferProcessing(imageBytes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing TWAIN data transfer");
                ScanError?.Invoke(this, new ScanErrorEventArgs { Exception = ex, Message = ex.Message });
            }
        }

        private Task ProcessTransferredImageAsync(byte[] imageBytes)
        {
            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), $"scan_{DateTime.Now.Ticks}_{_pageCounter}.jpg");

                using (var memory = new MemoryStream(imageBytes))
                using (var img = Image.FromStream(memory))
                {
                    using var reducedImage = ReduceResolution((Bitmap)img, _currentOptions.Dpi);
                    reducedImage.Save(tempPath, _jpegEncoder, _jpegParameters);
                }

                _scannedImages.Add(tempPath);
                bool isFrontSide = _pageCounter % 2 == 0;

                try
                {
                    ImageScanned?.Invoke(this, new ImageScannedEventArgs
                    {
                        ImagePath = tempPath,
                        PageNumber = _pageCounter + 1,
                        IsFrontSide = isFrontSide,
                        ScannedAt = DateTime.Now
                    });
                }
                catch (Exception subEx)
                {
                    _logger.LogError(subEx, "[TwainScanner] Exception thrown by ImageScanned subscriber");
                }

                _pageCounter++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing TWAIN data transfer");
                ScanError?.Invoke(this, new ScanErrorEventArgs { Exception = ex, Message = ex.Message });
            }
            finally
            {
                CompleteTransferProcessing();
            }

            return Task.CompletedTask;
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
            //  Debug.WriteLine($"[Thread {Environment.CurrentManagedThreadId}] Transfer ready");

            ScanProgress?.Invoke(this, new ScanProgressEventArgs
            {
                PagesScanned = _pageCounter,
                Message = "Ready to transfer image..."
            });
        }

        private void OnTransferError(object sender, TransferErrorEventArgs e)
        {
            //Debug.WriteLine($"[Thread {Environment.CurrentManagedThreadId}] Transfer error: {e.Exception?.Message ?? e.Code.ToString()}");

            _logger.LogError(e.Exception, "TWAIN transfer error");
            ScanError?.Invoke(this, new ScanErrorEventArgs
            {
                Exception = e.Exception,
                Message = e.Exception?.Message ?? e.Code.ToString()
            });
        }

        private void OnSourceDisabled(object sender, TWIdentityWrapper e)
        {
            _isScanning = false;
            _sourceDisabledRaised = true;
            _sourceDisabledTcs?.TrySetResult(true);
            TryCompleteTransfers();

            ScanCompleted?.Invoke(this, new ScanCompletedEventArgs
            {
                TotalPages = _pageCounter,
                ImagePaths = new List<string>(_scannedImages),
                Duration = TimeSpan.Zero
            });
        }

        private void OnStateChanged(object sender, STATE state)
        {
            //  Debug.WriteLine($"State changed to: {state}");
            _logger.LogInfo($"TWAIN State: {state}");
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            _jpegParameters?.Dispose();
            _ = DisconnectAsync();
            _twainThread.Dispose();
        }

        private void DisconnectInternal()
        {
            if (_twainSession != null)
            {
                _twainSession.StateChanged -= OnStateChanged;
                _twainSession.SourceDisabled -= OnSourceDisabled;
                _twainSession.TransferReady -= OnTransferReady;
                _twainSession.Transferred -= OnTransferred;
                _twainSession.TransferError -= OnTransferError;

                if (_twainSession.State >= STATE.S4)
                {
                    _twainSession.CloseSource();
                }

                if (_twainSession.State >= STATE.S3)
                {
                    _twainSession.CloseDsm();
                }

                _twainSession.Dispose();
                _twainSession = null;
            }

            _currentSource = null;
            _isScanning = false;
            _sourceDisabledTcs?.TrySetResult(true);
            _transfersCompletedTcs?.TrySetResult(true);
        }

        private static TaskCompletionSource<bool> CreateCompletionSource(bool completed = false)
        {
            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            if (completed)
            {
                tcs.TrySetResult(true);
            }

            return tcs;
        }

        private void BeginTransferProcessing()
        {
            lock (_transferSync)
            {
                if (_activeTransfers == 0)
                {
                    _transfersCompletedTcs = CreateCompletionSource();
                }

                _activeTransfers++;
            }
        }

        private void QueueTransferProcessing(byte[] imageBytes)
        {
            lock (_transferSync)
            {
                _transferProcessingChain = _transferProcessingChain.ContinueWith(
                    _ => ProcessTransferredImageAsync(imageBytes),
                    CancellationToken.None,
                    TaskContinuationOptions.None,
                    TaskScheduler.Default).Unwrap();
            }
        }

        private void CompleteTransferProcessing()
        {
            lock (_transferSync)
            {
                if (_activeTransfers > 0)
                {
                    _activeTransfers--;
                }
            }

            TryCompleteTransfers();
        }

        private void TryCompleteTransfers()
        {
            lock (_transferSync)
            {
                if (_sourceDisabledRaised && _activeTransfers == 0)
                {
                    _transfersCompletedTcs?.TrySetResult(true);
                }
            }
        }

        private sealed class TwainStaThread : IDisposable
        {
            private readonly Thread _thread;
            private readonly TaskCompletionSource<SynchronizationContext> _readyTcs =
                new(TaskCreationOptions.RunContinuationsAsynchronously);
            private SynchronizationContext _context;
            private bool _disposed;

            public TwainStaThread()
            {
                _thread = new Thread(ThreadMain)
                {
                    IsBackground = true,
                    Name = "Twain STA Thread"
                };
                _thread.SetApartmentState(ApartmentState.STA);
                _thread.Start();
            }

            public async Task InvokeAsync(Action action)
            {
                ArgumentNullException.ThrowIfNull(action);
                var context = await GetContextAsync().ConfigureAwait(false);
                var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

                context.Post(_ =>
                {
                    try
                    {
                        action();
                        tcs.TrySetResult(true);
                    }
                    catch (Exception ex)
                    {
                        tcs.TrySetException(ex);
                    }
                }, null);

                await tcs.Task.ConfigureAwait(false);
            }

            public async Task<T> InvokeAsync<T>(Func<T> func)
            {
                ArgumentNullException.ThrowIfNull(func);
                var context = await GetContextAsync().ConfigureAwait(false);
                var tcs = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);

                context.Post(_ =>
                {
                    try
                    {
                        tcs.TrySetResult(func());
                    }
                    catch (Exception ex)
                    {
                        tcs.TrySetException(ex);
                    }
                }, null);

                return await tcs.Task.ConfigureAwait(false);
            }

            public void Dispose()
            {
                if (_disposed)
                {
                    return;
                }

                _disposed = true;
                if (_context != null)
                {
                    _context.Post(_ => Application.ExitThread(), null);
                }
            }

            private async Task<SynchronizationContext> GetContextAsync()
            {
                return _context ?? await _readyTcs.Task.ConfigureAwait(false);
            }

            private void ThreadMain()
            {
                SynchronizationContext.SetSynchronizationContext(new WindowsFormsSynchronizationContext());
                _context = SynchronizationContext.Current ?? new WindowsFormsSynchronizationContext();
                _readyTcs.TrySetResult(_context);
                Application.Run();
            }
        }
    }
}
