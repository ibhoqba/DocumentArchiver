using System;
using System.IO;
using System.Text.RegularExpressions;

namespace DocumentArchiever.Helpers
{
    public static class PathHelper
    {
        public static string GetSafeFileName(string fileName)
        {
            string invalidChars = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            return Regex.Replace(fileName, $"[{Regex.Escape(invalidChars)}]", "_");
        }

        public static string GetDocumentPath(int year, int branchId, int month, int day, string docType, string documentNumber)
        {
            return Path.Combine(year.ToString(), branchId.ToString(), month.ToString(), day.ToString(), docType, $"{documentNumber}.pdf");
        }

        public static string GetFullDocumentPath(string basePath, string relativePath)
        {
            return Path.Combine(basePath, relativePath);
        }

        public static string EnsureTrailingBackslash(string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;

            return path.EndsWith("\\") ? path : path + "\\";
        }
    }
}