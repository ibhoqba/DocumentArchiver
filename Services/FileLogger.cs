using System;
using System.IO;
using System.Threading;

namespace DocumentArchiever.Services
{
    public class FileLogger : ILogger, IDisposable
    {
        private readonly string _logDirectory;
        private readonly string _errorDirectory;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private readonly int _maxFileSizeBytes = 512 * 1024;

        public FileLogger()
        {
            _logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            _errorDirectory = Path.Combine(_logDirectory, "Errors");

            Directory.CreateDirectory(_logDirectory);
            Directory.CreateDirectory(_errorDirectory);
        }

        public void LogInfo(string message)
        {
            WriteLog(Path.Combine(_logDirectory, $"trace_{DateTime.Now:yyyyMMdd}.log"),
                $"[INFO] {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - {message}");
        }

        public void LogWarning(string message)
        {
            WriteLog(Path.Combine(_logDirectory, $"trace_{DateTime.Now:yyyyMMdd}.log"),
                $"[WARN] {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - {message}");
        }

        public void LogError(string message)
        {
            WriteLog(Path.Combine(_errorDirectory, $"error_{DateTime.Now:yyyyMMdd}.log"),
                $"[ERROR] {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - {message}");
        }

        public void LogError(Exception ex, string context = null)
        {
            var message = context != null
                ? $"{context}: {ex.Message}\nStack Trace: {ex.StackTrace}"
                : $"{ex.Message}\nStack Trace: {ex.StackTrace}";

            LogError(message);
        }

        private void WriteLog(string filePath, string content)
        {
            _lock.EnterWriteLock();
            try
            {
                if (File.Exists(filePath) && new FileInfo(filePath).Length > _maxFileSizeBytes)
                {
                    File.WriteAllText(filePath, content + Environment.NewLine);
                }
                else
                {
                    File.AppendAllText(filePath, content + Environment.NewLine);
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public void Dispose()
        {
            _lock?.Dispose();
        }
    }
}