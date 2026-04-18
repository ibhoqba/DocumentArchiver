using System;
using System.Collections.Generic;

namespace DocumentArchiever.Data.Entities
{
    public class ScanSession
    {
        public string Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public long BranchId { get; set; }
        public string BranchName { get; set; }
        public int DocumentTypeId { get; set; }
        public int StartNumber { get; set; }
        public int CurrentNumber { get; set; }
        public int TotalScanned { get; set; }
        public List<int> MissedNumbers { get; set; }
        public bool IsContinuousMode { get; set; }
    }
}