
using BitMiracle.LibTiff.Classic;
using DocumentArchiever.Data.Entities;
using DocumentArchiever.Services;
using System.Data.SqlClient;

namespace DocumentArchiever.Data
{
    public class DatabaseService : IDatabaseService
    {
        private readonly string _connectionString;
        private readonly ILogger _logger;

        public DatabaseService(ILogger logger)
        {
            _logger = logger;
            _connectionString = BuildConnectionString();
        }

        private string BuildConnectionString()
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = Properties.Settings.Default.dbServer,
                InitialCatalog = Properties.Settings.Default.Database,
                UserID = Properties.Settings.Default.DbUserId,
                Password = Properties.Settings.Default.DbUserPwd,
                Encrypt = false,
                TrustServerCertificate = false,
                ConnectTimeout = 30
            };
            return builder.ConnectionString;
        }

        public async Task<List<Branch>> GetBranchesAsync()
        {
            var branches = new List<Branch>();
            const string query = "SELECT ID, BranchName FROM tblBranches WHERE ID <> 1 ORDER BY BranchName";

            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand(query, connection);

                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    branches.Add(new Branch
                    {
                        //Id = reader.GetInt32(0),
                        Id = reader.GetInt64(0),
                        Name = reader.GetString(1)
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load branches");
                throw;
            }

            return branches;
        }

        public async Task<Branch> GetBranchAsync(int id)
        {
            const string query = "SELECT ID, BranchName FROM tblBranches WHERE ID = @Id";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Branch
                {
                    Id = reader.GetInt64(0),
                    Name = reader.GetString(1)
                };
            }

            return null;
        }

        public async Task<bool> SaveDocumentAsync(Document document)
        {
            //_logger.LogInfo($"SaveDocumentAsync called for {document.DocumentNumber}");
            //_logger.LogInfo($"=== SAVE DOCUMENT START ===");
            //_logger.LogInfo($"DocumentNumber: {document.DocumentNumber}");
            //_logger.LogInfo($"FilePath: {document.FilePath}");
            //_logger.LogInfo($"FileName: {document.FileName}");
            //_logger.LogInfo($"BranchId: {document.BranchId}");
            //_logger.LogInfo($"DocumentTypeId: {document.DocumentTypeId}");
            //_logger.LogInfo($"Year: {document.Year}");

            const string checkQuery = "SELECT COUNT(1) FROM tblDocArchive WHERE TheNumber = @Number AND BranchID = @BranchId";
            const string insertQuery = @"INSERT INTO tblDocArchive (TheNumber, FilePath, FileName, BranchID, DocType, TheYear, EnterTime) 
                                         VALUES (@Number, @FilePath, @FileName, @BranchId, @DocType, @Year, GETDATE())";
            const string updateQuery = @"UPDATE tblDocArchive 
                                         SET FilePath = @FilePath, FileName = @FileName, EnterTime = GETDATE()
                                         WHERE TheNumber = @Number AND BranchID = @BranchId";

            if (!int.TryParse(document.DocumentNumber, out int number))
                return false;
            try
            {

                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var checkCmd = new SqlCommand(checkQuery, connection);
                checkCmd.Parameters.AddWithValue("@Number", number);
                checkCmd.Parameters.AddWithValue("@BranchId", document.BranchId);

                var exists = (int)await checkCmd.ExecuteScalarAsync() > 0;

                using var command = new SqlCommand(exists ? updateQuery : insertQuery, connection);
                command.Parameters.AddWithValue("@Number", number);
                command.Parameters.AddWithValue("@FilePath", document.FilePath ?? "");
                command.Parameters.AddWithValue("@FileName", document.FileName ?? "");
                command.Parameters.AddWithValue("@BranchId", document.BranchId);
                command.Parameters.AddWithValue("@DocType", document.DocumentTypeId);
                command.Parameters.AddWithValue("@Year", document.Year);

                var result = await command.ExecuteNonQueryAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database connection failed at SaveDocumentAsync");
                return false;

            }
        }

        public async Task<Document> GetDocumentAsync(string documentNumber, long branchId)
        {
            const string query = "SELECT TheNumber, FilePath, FileName, BranchID, DocType, TheYear, EnterTime " +
                                 "FROM tblDocArchive WHERE TheNumber = @Number AND BranchID = @BranchId";

            if (!int.TryParse(documentNumber, out int number))
                return null;

            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Number", number);
                command.Parameters.AddWithValue("@BranchId", branchId);

                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new Document
                    {
                        DocumentNumber = reader.GetInt64(0).ToString(),
                        FilePath = reader.GetString(1),
                        FileName = reader.GetString(2),
                        BranchId = reader.GetInt32(3),
                        DocumentTypeId = reader.GetInt32(4),
                        Year = reader.GetInt32(5),
                        CreatedAt = reader.GetDateTime(6)
                    };
                }

                return null;
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Database connection failed at GetDocumentAsync");
                return null;
            }
        }

        public async Task<int> GetLastDocumentNumberAsync(long branchId, int documentTypeId)
        {
            object result = null;

            try
            {
                const string query = "SELECT ISNULL(MAX(TheNumber), 0) FROM tblDocArchive WHERE BranchID = @BranchId AND DocType = @DocType";

                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@BranchId", branchId);
                command.Parameters.AddWithValue("@DocType", documentTypeId);

                await connection.OpenAsync();
                result = await command.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete documents");
                MessageBox.Show($"Value {result}\n{ex.Message} ");
                return -1;
            }
        }

        public async Task<bool> SaveScanSessionAsync(ScanSession session)
        {
            // Optional: Log scan sessions for auditing
            return await Task.FromResult(true);
        }
        /*
        public async Task<List<Document>> GetRecentDocumentsAsync(int count)
        {
            const string query = @"SELECT TOP(@Count) [ID], TheNumber, FilePath, FileName, BranchID, DocType, TheYear, EnterTime 
                                   FROM tblDocArchive 
                                   WHERE CAST(EnterTime AS DATE) = CAST(GETDATE() AS DATE)
                                   ORDER BY EnterTime DESC";

            try
            {
                var documents = new List<Document>();

                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Count", count);

                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    documents.Add(new Document
                    {
                        Id = (int)reader.GetInt64(0),
                        DocumentNumber = reader.GetInt64(1).ToString(),
                        FilePath = reader.GetString(2),
                        FileName = reader.GetString(3),
                        BranchId = reader.GetInt64(4),
                        DocumentTypeId = reader.GetInt32(5),
                        Year = (int)reader.GetInt16(6),
                        CreatedAt = reader.GetDateTime(7)
                    });
                }

                return documents;
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{ex.Message} \n{ex.StackTrace}");
                throw;
            }
        }
        */
        public async Task<List<Document>> GetRecentDocumentsAsync(int count)
        {
            const string query = @"SELECT TOP(@Count) [ID], TheNumber, FilePath, FileName, BranchID, DocType, TheYear, EnterTime 
                           FROM tblDocArchive 
                           WHERE CAST(EnterTime AS DATE) = CAST(GETDATE() AS DATE)
                           ORDER BY EnterTime DESC";

            var documents = new List<Document>();

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Count", count);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                documents.Add(new Document
                {
                    Id = (int)reader.GetInt64(0),
                    DocumentNumber = reader.IsDBNull(1) ? string.Empty : reader.GetInt64(1).ToString(),
                    FilePath = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    FileName = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                    BranchId = reader.IsDBNull(4) ? 0 : reader.GetInt64(4),
                    DocumentTypeId = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                    Year = reader.IsDBNull(6) ? 0 : (int)reader.GetInt16(6),
                    CreatedAt = reader.IsDBNull(7) ? DateTime.MinValue : reader.GetDateTime(7)
                });
            }

            return documents;
        }
        public async Task<bool> DeleteDocumentsAsync(List<int> ids)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            try
            {
                foreach (var id in ids)
                {
                    const string deleteQuery = "DELETE FROM tblDocArchive WHERE ID = @Id";
                    using var cmd = new SqlCommand(deleteQuery, connection, transaction);
                    cmd.Parameters.AddWithValue("@Id", id);
                    await cmd.ExecuteNonQueryAsync();
                }

                const string reseedQuery = "DBCC CHECKIDENT ('tblDocArchive', RESEED, (SELECT ISNULL(MAX(ID), 0) FROM tblDocArchive))";
                using var reseedCmd = new SqlCommand(reseedQuery, connection, transaction);
                await reseedCmd.ExecuteNonQueryAsync();

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError(ex, "Failed to delete documents");
                throw;
            }
        }

        public async Task<List<int>> GetYearsAsync()
        {
            var years = new List<int>();
            const string query = "SELECT DISTINCT TheYear FROM tblDocArchive WHERE TheYear IS NOT NULL ORDER BY TheYear DESC";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                years.Add(reader.GetInt32(0));
            }

            return years;
        }
    }
}