using System;

namespace DocumentArchiever.Data.Entities
{
    public class Document
    {
        public int Id { get; set; }
        public string DocumentNumber { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public long BranchId { get; set; }
        public int DocumentTypeId { get; set; }
        public int Year { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}