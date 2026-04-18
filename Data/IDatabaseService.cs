
using DocumentArchiever.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DocumentArchiever.Data
{
    public interface IDatabaseService
    {
        Task<List<Branch>> GetBranchesAsync();
        Task<Branch> GetBranchAsync(int id);
        Task<bool> SaveDocumentAsync(Document document);
        Task<Document> GetDocumentAsync(string documentNumber, long branchId);
        Task<int> GetLastDocumentNumberAsync(long branchId, int documentTypeId);
        Task<bool> SaveScanSessionAsync(ScanSession session);
        Task<List<Document>> GetRecentDocumentsAsync(int count);
        Task<bool> DeleteDocumentsAsync(List<int> ids);
        Task<List<int>> GetYearsAsync();
    }
}