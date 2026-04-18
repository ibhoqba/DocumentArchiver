using DocumentArchiever.Core.OCR;
using DocumentArchiever.Core.Processing;
using DocumentArchiever.Core.Scanning;
using DocumentArchiever.Data;
using DocumentArchiever.Data.Entities;
using DocumentArchiever.Services;
using DocumentArchiever.Setting;
using System.Threading;

namespace DocumentArchiever.UI.Forms
{
    public partial class MainForm : Form
    {
        private readonly IScanner _scanner;
        private readonly IOcrEngine _ocrEngine;
        private readonly IDatabaseService _databaseService;
        private readonly IUpdateService _updateService;
        private readonly ILogger _logger;
        private DocumentProcessor _documentProcessor;

        private CancellationTokenSource _scanCts;
        private ScanSession _currentSession;
        private List<string> _scannedImages;
        private List<string> _unidentifiedDocuments;
        private int _imageCounter;
        private bool _isDifferentTypeWarningShown;

        public MainForm(IScanner scanner, IOcrEngine ocrEngine, IDatabaseService databaseService,
                        IUpdateService updateService, ILogger logger)
        {
            InitializeComponent();
            Load += OnFormLoad;
            _scanner = scanner;
            _ocrEngine = ocrEngine;
            _databaseService = databaseService;
            _updateService = updateService;
            _logger = logger;

            _scannedImages = new List<string>();
            _unidentifiedDocuments = new List<string>();

            SubscribeToScannerEvents();

            FormClosing += OnFormClosing;
        }

        private void SubscribeToScannerEvents()
        {
            _scanner.ImageScanned += OnImageScanned;
            _scanner.ScanProgress += OnScanProgress;
            _scanner.ScanCompleted += OnScanCompleted;
            _scanner.ScanError += OnScanError;
        }

        private async void OnFormLoad(object sender, EventArgs e)
        {
            try
            {
                await LoadBranchesAsync();
                await LoadScannersAsync();
              
                InitializeDocumentProcessor();

                // Check for updates in background
                _ = Task.Run(() => _updateService.CheckForUpdatesAsync());
                LoadSavedSettings();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Form load failed");
                MessageBox.Show($"Failed to initialize: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void InitializeDocumentProcessor()
        {
            _documentProcessor = new DocumentProcessor(_ocrEngine, _databaseService, _logger);
            _documentProcessor.DocumentProcessed += OnDocumentProcessed;
            _documentProcessor.DocumentError += OnDocumentError;
            _documentProcessor.ProgressUpdated += OnProgressUpdated;
        }

        private async Task LoadScannersAsync()
        {
            try
            {
                var scanners = await _scanner.GetAvailableScannersAsync();
                cboScanners.DataSource = scanners;
                cboScanners.DisplayMember = "Name";
                cboScanners.ValueMember = "Id";

                btnScan.Enabled = scanners.Any();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load scanners");
                lblStatus.Text = $"خطأ في تحميل الماسحات: {ex.Message}";
            }
        }

        private async Task LoadBranchesAsync()
        {
            try
            {
                var branches = await _databaseService.GetBranchesAsync();

                // Set display/value members before binding to avoid binding issues
                cbBranches.DisplayMember = "Name";
                cbBranches.ValueMember = "Id";
                cbBranches.DataSource = branches;

                btnScan.Enabled = branches != null && branches.Any();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load branches");
                MessageBox.Show($"فشل تحميل الفروع: {ex.Message}", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadSavedSettings()
        {
            dtTime.Value = Properties.Settings.Default.LastDate != DateTime.MinValue
                ? Properties.Settings.Default.LastDate
                : DateTime.Now;

            txtDPI.Text = "150";

            // Convert saved Int32 to Int64 to match Branch.Id type
            if (cbBranches.DataSource != null && Properties.Settings.Default.LastSelectedBranch > 0)
            {
                cbBranches.SelectedValue = Properties.Settings.Default.LastSelectedBranch;
            }
        }
        
        /*
        private void LoadSavedSettings()
        {
            // Load from saved settings
            dtTime.Value = Properties.Settings.Default.LastDate != DateTime.MinValue
                ? Properties.Settings.Default.LastDate
                : DateTime.Now;

            txtDPI.Text = Properties.Settings.Default.LastDpi > 0
                ? Properties.Settings.Default.LastDpi.ToString()
                : "150";

            // Load last selected branch
            if (Properties.Settings.Default.LastSelectedBranch > 0)
            {
                // Wait for branches to load then select
                cbBranches.SelectedValue = Properties.Settings.Default.LastSelectedBranch;
            }

            // Load document type
            cbCashDocs.Checked = Properties.Settings.Default.LastDocumentType == 18;
            cbBtwBranches.Checked = Properties.Settings.Default.LastDocumentType == 91;

            // Load scanner settings
            chkDuplex.Checked = Properties.Settings.Default.LastDuplexEnabled;
            rbColor.Checked = Properties.Settings.Default.LastColorMode == "Color";
            rbGrayscale.Checked = Properties.Settings.Default.LastColorMode == "Grayscale";
            rbBlackWhite.Checked = Properties.Settings.Default.LastColorMode == "BlackWhite";
            chkContinuousScan.Checked = Properties.Settings.Default.LastContinuousScan;
        }
        */
        private void SaveSettings()
        {
            Properties.Settings.Default.LastDate = dtTime.Value;
           // Properties.Settings.Default.LastDpi = int.TryParse(txtDPI.Text, out var dpi) ? dpi : 150;

            if (cbBranches.SelectedValue != null)
            {
                Properties.Settings.Default.LastSelectedBranch =(Int64) cbBranches.SelectedValue;
            }

            Properties.Settings.Default.LastDocumentType = GetSelectedDocumentType();
            //Properties.Settings.Default.LastDuplexEnabled = chkDuplex.Checked;

          //  if (rbColor.Checked) Properties.Settings.Default.LastColorMode = "Color";
          // else if (rbGrayscale.Checked) Properties.Settings.Default.LastColorMode = "Grayscale";
          //  else Properties.Settings.Default.LastColorMode = "BlackWhite";

          //  Properties.Settings.Default.LastContinuousScan = chkContinuousScan.Checked;
            Properties.Settings.Default.Save();
        }

      
        private void OnImageScanned(object sender, ImageScannedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, ImageScannedEventArgs>(OnImageScanned), sender, e);
                return;
            }

            _scannedImages.Add(e.ImagePath);
            _imageCounter++;
            _logger.LogInfo($"Image scanned: {e.ImagePath}, Total: {_imageCounter}");
            // Process images in pairs for duplex mode
            if (chkDuplex.Checked && _scannedImages.Count % 2 == 0 && _scannedImages.Count >= 2)
            {
                _logger.LogInfo("Processing document pair...");
                ProcessDocumentPair();
            }
            else if (!chkDuplex.Checked && _scannedImages.Count >= 1)
            {
                ProcessSingleDocument();
            }

            UpdateInfo($"تم مسح {_imageCounter} صورة");
        }

        private async void ProcessDocumentPair()
        {
            var frontImage = _scannedImages[_scannedImages.Count - 2];
            var backImage = _scannedImages[_scannedImages.Count - 1];

            if (await IsBlankImageAsync(frontImage) && await IsBlankImageAsync(backImage))
            {
                _logger.LogWarning("Both sides blank, skipping");
                CleanupTempFiles(frontImage, backImage);
                return;
            }

            // Validate document type
            if (!_isDifferentTypeWarningShown && !await ValidateDocumentTypeAsync(frontImage))
            {
                var result = MessageBox.Show(
                    $"يرجى التأكد من نوع المستند. هل تريد متابعة التصوير؟",
                    "تحذير: نوع المستند",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                _isDifferentTypeWarningShown = true;

                if (result == DialogResult.No)
                {
                    CleanupTempFiles(frontImage, backImage);
                    return;
                }
            }

            string documentNumber = await GetDocumentNumberAsync(frontImage);

            if (string.IsNullOrEmpty(documentNumber))
            {
                _unidentifiedDocuments.Add(frontImage);
                if (!string.IsNullOrEmpty(backImage))
                    _unidentifiedDocuments.Add(backImage);
                return;
            }

            _documentProcessor.QueueDocument(frontImage, backImage, documentNumber);
            _documentProcessor.QueueMissedNumbers();

            if (IsNumberedDocument())
            {
                _currentSession.CurrentNumber++;
            }
        }

        private async void ProcessSingleDocument()
        {
            var image = _scannedImages.Last();

            if (await IsBlankImageAsync(image))
            {
                CleanupTempFiles(image);
                return;
            }

            string documentNumber = await GetDocumentNumberAsync(image);

            if (string.IsNullOrEmpty(documentNumber))
            {
                _unidentifiedDocuments.Add(image);
                return;
            }

            _documentProcessor.QueueDocument(image, null, documentNumber);
            _documentProcessor.QueueMissedNumbers();

            if (IsNumberedDocument())
            {
                _currentSession.CurrentNumber++;
            }
        }

        private async Task<string> GetDocumentNumberAsync(string imagePath)
        {
            if (IsNumberedDocument())
            {
                return _currentSession.CurrentNumber.ToString();
            }

            string expressNumber = await _ocrEngine.ExtractExpressNumberAsync(imagePath);

            if (string.IsNullOrEmpty(expressNumber) && !chkWithoutIdentity.Checked)
            {
                expressNumber = await _ocrEngine.ExtractVoucherNumberAsync(imagePath);
            }

            return expressNumber;
        }

        private async Task<bool> ValidateDocumentTypeAsync(string imagePath)
        {
            var docType = await _ocrEngine.IdentifyDocumentTypeAsync(imagePath);
            int expectedType = GetSelectedDocumentType();

            return expectedType switch
            {
                9 => docType == DocumentType.TransferVoucher,
                18 => docType == DocumentType.CashVoucher,
                91 => docType == DocumentType.InterBranchCash,
                _ => true
            };
        }

        private async Task<bool> IsBlankImageAsync(string imagePath)
        {
            return await Task.Run(() =>
            {
                try
                {
                    using (var image = (Bitmap)Image.FromFile(imagePath))
                    {
                        int whitePixels = 0;
                        int totalPixels = 0;

                        for (int y = 0; y < image.Height; y += 10)
                        {
                            for (int x = 0; x < image.Width; x += 10)
                            {
                                Color pixel = image.GetPixel(x, y);
                                double luminance = 0.299 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B;
                                if (luminance >= 220)
                                    whitePixels++;
                                totalPixels++;
                            }
                        }

                        return totalPixels == 0 || (double)whitePixels / totalPixels >= 0.95;
                    }
                }
                catch
                {
                    return true;
                }
            });
        }

        private bool IsNumberedDocument()
        {
            return cbCashDocs.Checked || cbBtwBranches.Checked;
        }

        private int GetSelectedDocumentType()
        {
            if (cbCashDocs.Checked) return 18;
            if (cbBtwBranches.Checked) return 91;
            return 9;
        }

        private void OnScanProgress(object sender, ScanProgressEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, ScanProgressEventArgs>(OnScanProgress), sender, e);
                return;
            }

            UpdateInfo(e.Message);
        }
        private void OnScanCompleted(object sender, ScanCompletedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, ScanCompletedEventArgs>(OnScanCompleted), sender, e);
                return;
            }

            // Stop progress bar
            progressBar1.Visible = false;
            progressBar1.Style = ProgressBarStyle.Blocks;

            _documentProcessor.ProcessMissedNumbersRemaining();
            _documentProcessor.StopProcessing();

            if (_unidentifiedDocuments.Count > 0)
            {
                ShowUnidentifiedDocuments();
            }

            ResetScanUI();
            UpdateInfo($"اكتمل المسح. تم مسح {e.TotalPages} صورة");

            if (chkContinuousScan.Checked && _unidentifiedDocuments.Count == 0)
            {
                PromptForMoreDocuments();
            }
        }

        /*
        private void OnScanCompleted(object sender, ScanCompletedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, ScanCompletedEventArgs>(OnScanCompleted), sender, e);
                return;
            }

            _documentProcessor.ProcessMissedNumbersRemaining();
            _documentProcessor.StopProcessing();

            if (_unidentifiedDocuments.Count > 0)
            {
                ShowUnidentifiedDocuments();
            }

            ResetScanUI();
            UpdateInfo($"اكتمل المسح. تم مسح {e.TotalPages} صورة");

            if (chkContinuousScan.Checked)
            {
                PromptForMoreDocuments();
            }
        }
        */
        private void OnScanError(object sender, ScanErrorEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, ScanErrorEventArgs>(OnScanError), sender, e);
                return;
            }

            _logger.LogError(e.Exception, e.Message);
            MessageBox.Show($"خطأ في المسح: {e.Message}", "خطأ",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            ResetScanUI();
        }

        private void OnDocumentProcessed(object sender, DocumentProcessedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, DocumentProcessedEventArgs>(OnDocumentProcessed), sender, e);
                return;
            }

            UpdateInfo($"تم حفظ المستند {e.DocumentNumber}");
        }

        private void OnDocumentError(object sender, DocumentErrorEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, DocumentErrorEventArgs>(OnDocumentError), sender, e);
                return;
            }

            _logger.LogError($"فشل حفظ المستند {e.DocumentNumber}: {e.Error}");
            AddError($"خطأ في المستند {e.DocumentNumber}: {e.Error}");
        }

        private void OnProgressUpdated(object sender, ProgressEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, ProgressEventArgs>(OnProgressUpdated), sender, e);
                return;
            }

            lblInfo.Text = e.Message;
        }

        private async void btnScan_Click(object sender, EventArgs e)
        {
            if (!ValidateScanSettings())
                return;

            try
            {
                _scanCts = new CancellationTokenSource();
                btnScan.Enabled = false;
                btnCancel.Enabled = true;
                progressBar1.Style = ProgressBarStyle.Marquee;
                progressBar1.Visible = true;

                _scannedImages.Clear();
                _unidentifiedDocuments.Clear();
                _imageCounter = 0;
                _isDifferentTypeWarningShown = false;

                _currentSession = CreateScanSession();
                _documentProcessor.InitializeSession(_currentSession);

                var selectedScanner = (ScannerInfo)cboScanners.SelectedItem;
                var options = new ScanOptions
                {
                    Dpi = int.TryParse(txtDPI.Text, out var dpi) ? dpi : 150,
                    ColorMode = GetSelectedColorMode(),
                    DuplexEnabled = chkDuplex.Checked,
                    UseFeeder = true,
                    ContinuousMode = chkContinuousScan.Checked
                };

                await _scanner.ConnectAsync(selectedScanner, _scanCts.Token);
                await _scanner.StartScanningAsync(options, _scanCts.Token);
            }
            catch (OperationCanceledException)
            {
                UpdateInfo("تم إلغاء المسح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Scan failed");
                MessageBox.Show($"فشل المسح: {ex.Message}", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                ResetScanUI();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _scanCts?.Cancel();
            _scanner.StopScanningAsync().Wait();
            UpdateInfo("جاري إلغاء المسح...");
        }

        private void btnRefreshScanners_Click(object sender, EventArgs e)
        {
            _ = LoadScannersAsync();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            ResetUI();
        }
        private ScanSession CreateScanSession()
        {
            var missedNumbers = ParseMissedNumbers(txtMissedNumbers.Text);
            int startNumber = IsNumberedDocument() && int.TryParse(txtStartDocNumber.Text, out var num) ? num : 1;

          

            var session = new ScanSession
            {
                Id = Guid.NewGuid().ToString(),
                StartTime = dtTime.Value,
                BranchId = (Int64)cbBranches.SelectedValue,//Convert.ToInt64(cbBranches.SelectedValue),
                BranchName = cbBranches.Text,  // ← Store the name
                DocumentTypeId = GetSelectedDocumentType(),
                StartNumber = startNumber,
                CurrentNumber = startNumber,
                MissedNumbers = missedNumbers,
                IsContinuousMode = chkContinuousScan.Checked
            };

            CreateSessionFolders(session);
            return session;
        }
       
        private void CreateSessionFolders(ScanSession session)
        {
            try
            {
                string branchName = cbBranches.Text;
                branchName = DocumentProcessor.SanitizeFolderName(branchName);

                string savePath = Path.Combine(
                    Properties.Settings.Default.SavePath,
                    session.StartTime.Year.ToString(),
                    branchName,  // ← Use BranchName
                    session.StartTime.Month.ToString(),
                    session.StartTime.Day.ToString(),
                    GetDocumentTypeName());

                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                   // _logger.LogInfo($"Created folder: {savePath}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create session folders");
                MessageBox.Show($"فشل إنشاء مجلد الحفظ: {ex.Message}", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /*
        private void CreateSessionFolders(ScanSession session)
        {
            try
            {
                string savePath = GetDocumentSavePath(session);
                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                    _logger.LogInfo($"Created folder: {savePath}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create session folders");
                MessageBox.Show($"فشل إنشاء مجلد الحفظ: {ex.Message}", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        */
        private string GetDocumentSavePath(ScanSession session = null)
        {
            if (session == null)
            {
                session = _currentSession;
                if (session == null) return Properties.Settings.Default.SavePath;
            }

            string docTypeName = session.DocumentTypeId switch
            { 
                18 => "سندات صرف نقدي",
                91 => "سندات صرف بين الفروع",
                _ => "سندات صرف حوالات"
            };

            return Path.Combine(
                Properties.Settings.Default.SavePath,
                session.StartTime.Year.ToString(),
                session.BranchId.ToString(),
                session.StartTime.Month.ToString(),
                session.StartTime.Day.ToString(),
                docTypeName);
        }
        /*
        private ScanSession CreateScanSession()
        {
            var missedNumbers = ParseMissedNumbers(txtMissedNumbers.Text);
            int startNumber = IsNumberedDocument() && int.TryParse(txtStartDocNumber.Text, out var num) ? num : 1;

            return new ScanSession
            {
                Id = Guid.NewGuid().ToString(),
                StartTime = dtTime.Value,
                BranchId = Convert.ToInt32(cbBranches.SelectedValue),
                DocumentTypeId = GetSelectedDocumentType(),
                StartNumber = startNumber,
                CurrentNumber = startNumber,
                MissedNumbers = missedNumbers,
                IsContinuousMode = chkContinuousScan.Checked
            };
        }
        */
        private List<int> ParseMissedNumbers(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return new List<int>();

            return input.Split('.')
                .Where(p => int.TryParse(p.Trim(), out _))
                .Select(p => int.Parse(p.Trim()))
                .ToList();
        }

        private ColorMode GetSelectedColorMode()
        {
            if (rbBlackWhite.Checked) return ColorMode.BlackWhite;
            if (rbGrayscale.Checked) return ColorMode.Grayscale;
            return ColorMode.Color;
        }

        private bool ValidateScanSettings()
        {
            if (cboScanners.SelectedItem == null)
            {
                MessageBox.Show("يرجى اختيار الماسح الضوئي", "تحقق",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (cbBranches.SelectedItem == null)
            {
                MessageBox.Show("يرجى اختيار الفرع", "تحقق",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (IsNumberedDocument())
            {
                if (!int.TryParse(txtStartDocNumber.Text, out _))
                {
                    MessageBox.Show("يرجى إدخال رقم بداية صحيح", "تحقق",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            return true;
        }
        private void ResetScanUI()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(ResetScanUI));
                return;
            }

            btnScan.Enabled = true;
            btnCancel.Enabled = false;
            progressBar1.Visible = false;
            progressBar1.Style = ProgressBarStyle.Blocks;
            cboScanners.Enabled = true;
            _scanCts?.Dispose();
        }
        /*
        private void ResetScanUI()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(ResetScanUI));
                return;
            }

            btnScan.Enabled = true;
            btnCancel.Enabled = false;
            progressBar1.Visible = false;
            cboScanners.Enabled = true;
            _scanCts?.Dispose();
        }
        */
        private void ResetUI()
        {
            _scannedImages.Clear();
            _unidentifiedDocuments.Clear();
            _imageCounter = 0;
            lblErrors.Text = "";
            UpdateInfo("جاهز");

            if (IsNumberedDocument() && _currentSession != null)
            {
                txtStartDocNumber.Text = _currentSession.CurrentNumber.ToString();
            }
        }

        private void ShowUnidentifiedDocuments()
        {
            pnlSearch.Visible = true;
            lstUnidentified.Items.Clear();
            foreach (var doc in _unidentifiedDocuments)
            {
                lstUnidentified.Items.Add(Path.GetFileName(doc));
            }
            btnGetAllExpress.Visible = true;
            BtnRandomSave.Visible = true;
        }

        private void PromptForMoreDocuments()
        {
            var result = MessageBox.Show("هل تريد مسح مستندات إضافية؟", "مواصلة المسح",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                _scannedImages.Clear();
                _imageCounter = 0;
                btnScan.PerformClick();
            }
        }

        private void CleanupTempFiles(params string[] paths)
        {
            foreach (var path in paths)
            {
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    try { File.Delete(path); } catch { }
                }
            }
        }

        private void UpdateInfo(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(UpdateInfo), message);
                return;
            }

            lblInfo.Text = message;
            _logger.LogInfo(message);
        }

        private void AddError(string error)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(AddError), error);
                return;
            }

            lblErrors.Text += $"\n{error}";
        }



        // Event handlers for UI controls
        private void cbCashDocs_CheckedChanged(object sender, EventArgs e)
        {
            txtStartDocNumber.Visible = cbCashDocs.Checked;
            txtMissedNumbers.Visible = cbCashDocs.Checked;
            lblDocStartNumber.Visible = cbCashDocs.Checked;

            if (cbCashDocs.Checked)
            {
                cbBtwBranches.Checked = false;
            }
        }

        private void cbBtwBranches_CheckedChanged(object sender, EventArgs e)
        {
            txtBtwBranch.Visible = cbBtwBranches.Checked;
            txtMissedNumbers.Visible = cbBtwBranches.Checked;
            lblDocStartNumber.Visible = cbBtwBranches.Checked;

            if (cbBtwBranches.Checked)
            {
                cbCashDocs.Checked = false;
            }
        }

        private async void cbBranches_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbBranches.SelectedValue != null && IsNumberedDocument())
            {
                long branchId = (Int64) cbBranches.SelectedValue;
                int docType = GetSelectedDocumentType();
                int lastNumber = await _databaseService.GetLastDocumentNumberAsync(branchId, docType);

                if (cbCashDocs.Checked)
                    txtStartDocNumber.Text = (lastNumber + 1).ToString();
                else if (cbBtwBranches.Checked)
                    txtBtwBranch.Text = (lastNumber + 1).ToString();
            }
        }

        private void txtStartDocNumber_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(txtStartDocNumber.Text, out int num))
            {
                if (_currentSession != null)
                    _currentSession.CurrentNumber = num;
            }
        }

        private void lstUnidentified_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstUnidentified.SelectedItem != null)
            {
                string selectedFile = lstUnidentified.SelectedItem.ToString();
                string fullPath = Path.Combine(Properties.Settings.Default.SavePath, selectedFile);

                if (File.Exists(fullPath))
                {
                    webBrowserPdf.Navigate(fullPath);
                    btnSaveToDatabase.Visible = true;
                    btnSearch.Visible = false;
                }
            }
        }

        private async void btnSaveToDatabase_Click(object sender, EventArgs e)
        {
            if (lstUnidentified.SelectedItem == null)
            {
                MessageBox.Show("يرجى اختيار مستند", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string documentNumber = txtUniqueNumber.Text.Trim();
            if (string.IsNullOrEmpty(documentNumber) || !documentNumber.All(char.IsDigit))
            {
                MessageBox.Show("يرجى إدخال رقم مستند صحيح", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedFile = lstUnidentified.SelectedItem.ToString();
            string oldPath = Path.Combine(Properties.Settings.Default.SavePath, selectedFile);
            string newPath = Path.Combine(GetDocumentSavePath(), $"{documentNumber}.pdf");

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(newPath));

                if (File.Exists(oldPath))
                {
                    File.Move(oldPath, newPath);
                }

                var document = new Document
                {
                    DocumentNumber = documentNumber,
                    FilePath = GetRelativePath(),
                    FileName = $"{documentNumber}.pdf",
                    BranchId = (int)cbBranches.SelectedValue,
                    DocumentTypeId = GetSelectedDocumentType(),
                    Year = dtTime.Value.Year,
                    CreatedAt = DateTime.Now
                };

                bool saved = await _databaseService.SaveDocumentAsync(document);

                if (saved)
                {
                    lstUnidentified.Items.Remove(selectedFile);
                    txtUniqueNumber.Text = "";
                    UpdateInfo($"تم حفظ المستند {documentNumber}");
                }
                else
                {
                    MessageBox.Show("فشل حفظ المستند في قاعدة البيانات", "خطأ",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save document manually");
                MessageBox.Show($"خطأ في الحفظ: {ex.Message}", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetDocumentSavePath()
        {
            return Path.Combine(
                Properties.Settings.Default.SavePath,
                dtTime.Value.Year.ToString(),
                cbBranches.SelectedValue?.ToString() ?? "1",
                dtTime.Value.Month.ToString(),
                dtTime.Value.Day.ToString(),
                GetDocumentTypeName());
        }

        private string GetRelativePath()
        {
            return Path.Combine(
                dtTime.Value.Year.ToString(),
                cbBranches.SelectedValue?.ToString() ?? "1",
                dtTime.Value.Month.ToString(),
                dtTime.Value.Day.ToString(),
                GetDocumentTypeName());
        }

        private string GetDocumentTypeName()
        {
            if (cbCashDocs.Checked) return "سندات صرف نقديv";
            if (cbBtwBranches.Checked) return "سندات صرف بين الفروع";
            return "سندات صرف حوالات";
        }
        //Unknown = 0,
        //"سندات صرف حوالات" = 1,
        // "سندات صرف بين الفروع" = 2,
        //  "سندات صرف نقدي" = 3
        private async void btnGetAllExpress_Click(object sender, EventArgs e)
        {
            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.Visible = true;
            btnGetAllExpress.Enabled = false;

            try
            {
                foreach (string item in lstUnidentified.Items)
                {
                    string fullPath = Path.Combine(Properties.Settings.Default.SavePath, item);
                    if (File.Exists(fullPath))
                    {
                        string expressNumber = await _ocrEngine.ExtractExpressNumberFromPdfAsync(fullPath);

                        if (!string.IsNullOrEmpty(expressNumber))
                        {
                            string newPath = Path.Combine(GetDocumentSavePath(), $"{expressNumber}.pdf");
                            Directory.CreateDirectory(Path.GetDirectoryName(newPath));
                            File.Move(fullPath, newPath);

                            var document = new Document
                            {
                                DocumentNumber = expressNumber,
                                FilePath = GetRelativePath(),
                                FileName = $"{expressNumber}.pdf",
                                BranchId = (int)cbBranches.SelectedValue,
                                DocumentTypeId = GetSelectedDocumentType(),
                                Year = dtTime.Value.Year,
                                CreatedAt = DateTime.Now
                            };

                            await _databaseService.SaveDocumentAsync(document);
                        }
                    }
                }

                await LoadScannersAsync();
                lstUnidentified.Items.Clear();
                pnlSearch.Visible = false;
                UpdateInfo("تم حفظ جميع المستندات");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process express numbers");
                MessageBox.Show($"خطأ: {ex.Message}", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                progressBar1.Visible = false;
                btnGetAllExpress.Enabled = true;
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            var searchForm = new SearchForm(_databaseService, _logger);
            searchForm.ShowDialog();
        }

        private void BtnEditDeleteLastScanned_Click(object sender, EventArgs e)
        {
            var editForm = new EditForm(_databaseService, _logger);
            editForm.ShowDialog();
        }

        private void BtnUploadFile_Click(object sender, EventArgs e)
        {
            BtnGetFile.Visible = !BtnGetFile.Visible;
        }

        private void BtnGetFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "اختر ملف PDF";
                openFileDialog.Filter = "PDF files (*.pdf)|*.pdf";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string fileName = Path.GetFileNameWithoutExtension(openFileDialog.FileName);

                    if (fileName.All(char.IsDigit))
                    {
                        string savePath = GetDocumentSavePath();
                        Directory.CreateDirectory(savePath);

                        string destPath = Path.Combine(savePath, Path.GetFileName(openFileDialog.FileName));
                        File.Copy(openFileDialog.FileName, destPath, true);

                        var document = new Document
                        {
                            DocumentNumber = fileName,
                            FilePath = GetRelativePath(),
                            FileName = Path.GetFileName(openFileDialog.FileName),
                            BranchId = (int)cbBranches.SelectedValue,
                            DocumentTypeId = GetSelectedDocumentType(),
                            Year = dtTime.Value.Year,
                            CreatedAt = DateTime.Now
                        };

                        _ = _databaseService.SaveDocumentAsync(document);
                        UpdateInfo($"تم حفظ المستند {fileName}");
                    }
                    else
                    {
                        MessageBox.Show("اسم الملف يجب أن يكون رقماً", "تنبيه",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private void BtnRandomSave_Click(object sender, EventArgs e)
        {
            _ = ProcessRandomDocumentsAsync();
        }

        private async Task ProcessRandomDocumentsAsync()
        {
            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.Visible = true;
            BtnRandomSave.Enabled = false;

            try
            {
                foreach (string item in lstUnidentified.Items)
                {
                    string fullPath = Path.Combine(Properties.Settings.Default.SavePath, item);
                    if (File.Exists(fullPath))
                    {
                        string number = await _ocrEngine.ExtractVoucherNumberAsync(fullPath);

                        if (!string.IsNullOrEmpty(number))
                        {
                            string newPath = Path.Combine(GetDocumentSavePath(), $"{number}.pdf");
                            Directory.CreateDirectory(Path.GetDirectoryName(newPath));
                            File.Move(fullPath, newPath);

                            var document = new Document
                            {
                                DocumentNumber = number,
                                FilePath = GetRelativePath(),
                                FileName = $"{number}.pdf",
                                BranchId = (int)cbBranches.SelectedValue,
                                DocumentTypeId = GetSelectedDocumentType(),
                                Year = dtTime.Value.Year,
                                CreatedAt = DateTime.Now
                            };

                            await _databaseService.SaveDocumentAsync(document);
                        }
                    }
                }

                lstUnidentified.Items.Clear();
                pnlSearch.Visible = false;
                UpdateInfo("تم حفظ جميع المستندات");
            }
            finally
            {
                progressBar1.Visible = false;
                BtnRandomSave.Enabled = true;
            }
        }


        private async void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings();
            // If there's an active scan, confirm cancellation
            if (_scanCts != null && _scanner.IsBusy)
            {
                var result = MessageBox.Show(
                    "المسح قيد التنفيذ. هل تريد إلغاء المسح وإغلاق التطبيق؟",
                    "تأكيد الإغلاق",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }

            // Show progress indicator
            var progressForm = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                Size = new System.Drawing.Size(200, 50),
                StartPosition = FormStartPosition.CenterParent,
                ControlBox = false,
                ShowInTaskbar = false
            };
            var progressLabel = new Label { Text = "جاري إنهاء العمليات...", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter };
            progressForm.Controls.Add(progressLabel);
            progressForm.Show(this);

            try
            {
                // Cancel scanning if in progress
              
                if (_scanCts != null && !_scanCts.IsCancellationRequested)
                {
                    try
                    {
                        _scanCts.Cancel();
                    }
                    catch (ObjectDisposedException)
                    {
                        // Already disposed, ignore
                    }
                }

                // Stop the scanner
                await _scanner.StopScanningAsync();
                await _scanner.DisconnectAsync();

                // Stop document processing with timeout
                if (_documentProcessor != null)
                {
                    var processingTask = Task.Run(() => _documentProcessor.StopProcessing());
                    var completed = await Task.WhenAny(processingTask, Task.Delay(5000));
                    if (completed != processingTask)
                    {
                        _logger.LogWarning("Document processor stop timeout - forcing shutdown");
                    }
                }

                // Dispose OCR engine
                _ocrEngine?.Dispose();

                // Save settings
                if (cbBranches.SelectedValue != null)
                {
                    Properties.Settings.Default.LastSelectedBranch = Convert.ToInt32(cbBranches.SelectedValue);
                }
                Properties.Settings.Default.LastDate = dtTime.Value;
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during form closing");
            }
            finally
            {
                progressForm.Close();
                progressForm.Dispose();
            }
        }

        private void tsmiSettings_Click(object sender, EventArgs e)
        {
            
                 var SettingForm = new fmDbSettings();
            SettingForm.ShowDialog();
        }
    }
}