
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
            _pdfGenerator = new PdfGenerator();
            _imageProcessor = new ImageProcessor(logger);
            _unidentifiedDocuments = new List<string>();
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
            _processingQueue = new BlockingCollection<DocumentPair>(new ConcurrentQueue<DocumentPair>());
            _processingCts = new CancellationTokenSource();
            _processingTask = Task.Run(() => ProcessQueueAsync(_processingCts.Token));
        }

        public void QueueDocument(string frontImage, string backImage, string documentNumber)
        {
            if (_processingQueue == null || _processingQueue.IsAddingCompleted)
            {
                InitializeProcessingQueue();
            }

            _processingQueue.Add(new DocumentPair
            {
                FrontImagePath = frontImage,
                BackImagePath = backImage,
                DocumentNumber = documentNumber
            });
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
            foreach (var pair in _processingQueue.GetConsumingEnumerable(cancellationToken))
            {
                try
                {
                    ProgressUpdated?.Invoke(this, new ProgressEventArgs
                    {
                        Message = $"Processing document {pair.DocumentNumber}",
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
                    _logger.LogError(ex, $"Failed to process document {pair.DocumentNumber}");
                    DocumentError?.Invoke(this, new DocumentErrorEventArgs
                    {
                        DocumentNumber = pair.DocumentNumber,
                        Error = ex.Message
                    });
                }
            }
        }

        private async Task SaveDocumentAsync(DocumentPair pair)
        {
            string savePath = GetDocumentSavePath();
            string pdfPath = Path.Combine(savePath, $"{pair.DocumentNumber}.pdf");

            Directory.CreateDirectory(savePath);

            await _pdfGenerator.GenerateAsync(pdfPath, pair.FrontImagePath, pair.BackImagePath);
            if (File.Exists(pdfPath))
            {
                _logger.LogInfo($"PDF created successfully at: {pdfPath}, Size: {new FileInfo(pdfPath).Length} bytes");
            }
            else
            {
                _logger.LogError($"PDF NOT created at: {pdfPath}");
               // throw new Exception("PDF file was not created");
            }
            var document = new Document
            {
                DocumentNumber = pair.DocumentNumber,
                FilePath = GetRelativePath(),
                FileName = $"{pair.DocumentNumber}.pdf",
                BranchId =_currentSession.BranchId,
                DocumentTypeId = _currentSession.DocumentTypeId,
                Year = _currentSession.StartTime.Year,
                CreatedAt = DateTime.Now
            };

            bool saved = await _databaseService.SaveDocumentAsync(document);

            if (!saved)
            {
                _unidentifiedDocuments.Add(pdfPath);
                throw new Exception("Failed to save to database");
            }
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
        /*
        private string GetDocumentSavePath()
        {
            return Path.Combine(
                Properties.Settings.Default.SavePath,
                _currentSession.StartTime.Year.ToString(),
                cb//_currentSession.BranchId.ToString(),
                _currentSession.StartTime.Month.ToString(),
                _currentSession.StartTime.Day.ToString(),
                GetDocumentTypeName());
        }*/
        private string GetDocumentSavePath()
        {
            // Get branch name from the selected branch in the UI
            string branchName = _currentSession.BranchName;// cbBranches.Text; // Gets the displayed BranchName

            // Sanitize branch name (remove invalid path characters)
            branchName = SanitizeFolderName(branchName);
            string ThePath=
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
        //private string GetRelativePath()
        //{
        //    return Path.Combine(
        //        _currentSession.StartTime.Year.ToString(),
        //        _currentSession.BranchId.ToString(),
        //        _currentSession.StartTime.Month.ToString(),
        //        _currentSession.StartTime.Day.ToString(),
        //        GetDocumentTypeName());
        //}
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
            _processingCts?.Cancel();
            _processingQueue?.CompleteAdding();
            _processingTask?.Wait(5000);
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