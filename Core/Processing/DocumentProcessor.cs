
using DocumentArchiever.Core.OCR;
using DocumentArchiever.Data;
using DocumentArchiever.Data.Entities;
using DocumentArchiever.Services;
using System.Collections.Concurrent;

namespace DocumentArchiever.Core.Processing
{
    public class DocumentProcessor
    {
        private readonly IOcrEngine _ocrEngine;
        private readonly IDatabaseService _databaseService;
        private readonly PdfGenerator _pdfGenerator;
        private readonly ImageProcessor _imageProcessor;
        private readonly ILogger _logger;

        private BlockingCollection<DocumentPair> _processingQueue;
        private CancellationTokenSource _processingCts;
        private Task _processingTask;
        private ScanSession _currentSession;
        private HashSet<int> _missedNumbers;
        private List<string> _unidentifiedDocuments;

        public event EventHandler<DocumentProcessedEventArgs> DocumentProcessed;
        public event EventHandler<DocumentErrorEventArgs> DocumentError;
        public event EventHandler<ProgressEventArgs> ProgressUpdated;

        public DocumentProcessor(IOcrEngine ocrEngine, IDatabaseService databaseService, ILogger logger)
        {
            _ocrEngine = ocrEngine;
            _databaseService = databaseService;
            _logger = logger;
            _pdfGenerator = new PdfGenerator(_logger);
            _imageProcessor = new ImageProcessor(logger);
            _unidentifiedDocuments = new List<string>();
        }
        public void QueueDocument(string frontImage, string backImage, string documentNumber)
        {
            //_logger.LogInfo($"QueueDocument called for {documentNumber}");
            //  _logger.LogInfo($"  Front: {frontImage}");
            //  _logger.LogInfo($"  Back: {backImage}");

            if (_processingQueue == null || _processingQueue.IsAddingCompleted)
            {
                _logger.LogWarning("Queue was null or completed, reinitializing...");
                InitializeProcessingQueue();
            }

            var pair = new DocumentPair
            {
                FrontImagePath = frontImage,
                BackImagePath = backImage,
                DocumentNumber = documentNumber,
                IsMissedDocument = false
            };

            _processingQueue.Add(pair);
            //  _logger.LogInfo($"Item added to queue. Total items: {_processingQueue.Count}");
        }
        public void InitializeSession(ScanSession session)
        {
            _currentSession = session;
            _missedNumbers = new HashSet<int>(session.MissedNumbers);
            _unidentifiedDocuments.Clear();

            InitializeProcessingQueue();
        }
        private void InitializeProcessingQueue()
        {
            //   _logger.LogInfo("InitializeProcessingQueue START");

            _processingQueue = new BlockingCollection<DocumentPair>(new ConcurrentQueue<DocumentPair>());
            _processingCts = new CancellationTokenSource();
            _processingTask = Task.Run(() => ProcessQueueAsync(_processingCts.Token));

            // _logger.LogInfo($"Processing task started. Task ID: {_processingTask.Id}");
        }



        public void QueueMissedNumbers()
        {
            while (_missedNumbers.Count > 0 && _missedNumbers.Min() == _currentSession.CurrentNumber)
            {

                _processingQueue.Add(new DocumentPair
                {
                    FrontImagePath = null,
                    BackImagePath = null,
                    DocumentNumber = _currentSession.CurrentNumber.ToString(),
                    IsMissedDocument = true
                });

                _missedNumbers.Remove(_currentSession.CurrentNumber);
                _currentSession.CurrentNumber++;

            }
        }
        //public void QueueMissedNumbers()
        //{
        //    while (_missedNumbers.Count > 0 && _missedNumbers.Min() == _currentSession.CurrentNumber)
        //    {
        //        int missed = _currentSession.CurrentNumber;

        //        _processingQueue.Add(new DocumentPair
        //        {
        //            FrontImagePath = null,
        //            BackImagePath = null,
        //            DocumentNumber = missed.ToString(),
        //            IsMissedDocument = true
        //        });

        //        _missedNumbers.Remove(missed);
        //        _currentSession.CurrentNumber = missed + 1;
        //    }
        //}
        public void ProcessMissedNumbersRemaining()
        {
            while (_missedNumbers.Count > 0)
            {
                int number = _missedNumbers.First();
                _processingQueue.Add(new DocumentPair
                {
                    FrontImagePath = null,
                    BackImagePath = null,
                    DocumentNumber = number.ToString(),
                    IsMissedDocument = true
                });
                _missedNumbers.Remove(number);
            }
        }


        private async Task ProcessQueueAsync(CancellationToken cancellationToken)
        {
            // _logger.LogInfo("ProcessQueueAsync started, waiting for items...");
            foreach (var pair in _processingQueue.GetConsumingEnumerable(cancellationToken))
            {
                try
                {
                    //    _logger.LogInfo($"Dequeued item: {pair.DocumentNumber}, IsMissed: {pair.IsMissedDocument}, Front: {pair.FrontImagePath}, Back: {pair.BackImagePath}");

                    ProgressUpdated?.Invoke(this, new ProgressEventArgs
                    {
                        Message = $"Processing queue item: {pair.DocumentNumber}",
                        Percentage = 0
                    });

                    if (pair.IsMissedDocument)
                    {
                        await SaveMissedDocumentAsync(pair);
                    }
                    else
                    {
                        await SaveDocumentAsync(pair);
                    }

                    DocumentProcessed?.Invoke(this, new DocumentProcessedEventArgs
                    {
                        DocumentNumber = pair.DocumentNumber,
                        Success = true
                    });

                    CleanupTempFiles(pair);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to process {pair.DocumentNumber}");
                    DocumentError?.Invoke(this, new DocumentErrorEventArgs
                    {
                        DocumentNumber = pair.DocumentNumber,
                        Error = ex.Message
                    });
                }
            }
            //   _logger.LogInfo("ProcessQueueAsync ended");
        }

        private async Task SaveDocumentAsync(DocumentPair pair)
        {
            string savePath = GetDocumentSavePath();
            string pdfPath = Path.Combine(savePath, $"{pair.DocumentNumber}.pdf");

            Directory.CreateDirectory(savePath);

            //  _logger.LogInfo($"SaveDocumentAsync START for {pair.DocumentNumber}. PDF path: {pdfPath}");
            // _logger.LogInfo($"  Front exists: {File.Exists(pair.FrontImagePath)}, Back exists: {File.Exists(pair.BackImagePath)}");

            try
            {
                // _logger.LogInfo($"Starting PDF generation for {pair.DocumentNumber} -> {pdfPath}");
                // If you want a dedicated single page method, switch here when BackImagePath is empty.
                await _pdfGenerator.GenerateAsync(pdfPath, pair.FrontImagePath, pair.BackImagePath);
                //    _logger.LogInfo($"PDF generation returned for {pair.DocumentNumber}");
            }
            catch (Exception genEx)
            {
                _logger.LogError(genEx, $"PDF generation failed for {pair.DocumentNumber} -> {pdfPath}");
                // ensure the failure is visible to caller and logged
                throw;
            }

            if (File.Exists(pdfPath))
            {
                _logger.LogInfo($"PDF created successfully at: {pdfPath}, Size: {new FileInfo(pdfPath).Length} bytes");
            }
            else
            {
                _logger.LogError($"PDF NOT created at: {pdfPath}");
                throw new Exception("PDF file was not created");
            }

            var document = new Document
            {
                DocumentNumber = pair.DocumentNumber,
                FilePath = GetRelativePath(),
                FileName = $"{pair.DocumentNumber}.pdf",
                BranchId = _currentSession.BranchId,
                DocumentTypeId = _currentSession.DocumentTypeId,
                Year = _currentSession.StartTime.Year,
                CreatedAt = DateTime.Now
            };

            _logger.LogInfo($"Attempting to save document metadata to DB for {pair.DocumentNumber}. FilePath: {document.FilePath}, FileName: {document.FileName}");
            bool saved = await _databaseService.SaveDocumentAsync(document);

            if (!saved)
            {
                _logger.LogError($"Database save returned false for {pair.DocumentNumber}");
                _unidentifiedDocuments.Add(pdfPath);
                throw new Exception("Failed to save to database");
            }
            _logger.LogInfo($"Document metadata saved to DB for {pair.DocumentNumber}");
        }


        private async Task SaveMissedDocumentAsync(DocumentPair pair)
        {
            var document = new Document
            {
                DocumentNumber = pair.DocumentNumber,
                FilePath = Properties.Settings.Default.SavePath,
                FileName = "a.pdf",
                BranchId = (int)_currentSession.BranchId,
                DocumentTypeId = _currentSession.DocumentTypeId,
                Year = _currentSession.StartTime.Year,
                CreatedAt = DateTime.Now
            };

            await _databaseService.SaveDocumentAsync(document);
        }

        private string GetDocumentSavePath()
        {
            // Get branch name from the selected branch in the UI
            string branchName = _currentSession.BranchName;// cbBranches.Text; // Gets the displayed BranchName

            // Sanitize branch name (remove invalid path characters)
            branchName = SanitizeFolderName(branchName);
            string ThePath =
             Path.Combine(
                Properties.Settings.Default.SavePath,
                _currentSession.StartTime.Year.ToString(),
                branchName,  // ← Use BranchName instead of BranchId
                _currentSession.StartTime.Month.ToString(),
                _currentSession.StartTime.Day.ToString(),
                GetDocumentTypeName());
            _logger.LogInfo($"Save path: {ThePath}");
            return ThePath;
        }

        public static string SanitizeFolderName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "Unknown";

            // Remove invalid characters for Windows folder names
            char[] invalidChars = Path.GetInvalidFileNameChars();
            foreach (char c in invalidChars)
            {
                name = name.Replace(c.ToString(), "");
            }

            // Also remove other problematic characters
            name = name.Replace(":", "")
                       .Replace("*", "")
                       .Replace("?", "")
                       .Replace("\"", "")
                       .Replace("<", "")
                       .Replace(">", "")
                       .Replace("|", "")
                       .Trim();

            return string.IsNullOrEmpty(name) ? "Unknown" : name;
        }

        private string GetRelativePath()
        {
            string branchName = _currentSession.BranchName;// cbBranches.Text;
            branchName = SanitizeFolderName(branchName);

            return Path.Combine(
                _currentSession.StartTime.Year.ToString(),
                branchName,  // ← Use BranchName instead of BranchId
                _currentSession.StartTime.Month.ToString(),
                _currentSession.StartTime.Day.ToString(),
                GetDocumentTypeName());
        }
        private string GetDocumentTypeName()
        {
            return _currentSession.DocumentTypeId switch
            {
                18 => "سندات صرف نقدي",
                91 => "سندات صرف بين الفروع",
                _ => "سندات صرف حوالات"
            };
        }

        private void CleanupTempFiles(DocumentPair pair)
        {
            if (!string.IsNullOrEmpty(pair.FrontImagePath) && File.Exists(pair.FrontImagePath))
            {
                TryDeleteFile(pair.FrontImagePath);
            }
            if (!string.IsNullOrEmpty(pair.BackImagePath) && File.Exists(pair.BackImagePath))
            {
                TryDeleteFile(pair.BackImagePath);
            }
        }

        private void TryDeleteFile(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Failed to delete temp file {path}: {ex.Message}");
            }
        }

        public List<string> GetUnidentifiedDocuments() => _unidentifiedDocuments;

        public void StopProcessing()
        {
            try
            {
                _logger.LogInfo("StopProcessing called - completing queue and waiting for remaining items to finish");
                _processingQueue?.CompleteAdding();
                _processingTask?.Wait(30000);
            }
            catch (AggregateException ex)
            {
                _logger.LogError(ex, "Error while waiting for processing queue to finish");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while stopping processing");
            }
        }

        private class DocumentPair
        {
            public string FrontImagePath { get; set; }
            public string BackImagePath { get; set; }
            public string DocumentNumber { get; set; }
            public bool IsMissedDocument { get; set; }
        }
    }

    public class DocumentProcessedEventArgs : EventArgs
    {
        public string DocumentNumber { get; set; }
        public bool Success { get; set; }
    }

    public class DocumentErrorEventArgs : EventArgs
    {
        public string DocumentNumber { get; set; }
        public string Error { get; set; }
    }

    public class ProgressEventArgs : EventArgs
    {
        public string Message { get; set; }
        public int Percentage { get; set; }
    }
}
