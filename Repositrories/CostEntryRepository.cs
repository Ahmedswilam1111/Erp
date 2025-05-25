using ERPtask.models;
using System.Data.SqlClient;

namespace ERPtask.Repositrories
{
    public class CostEntryRepository
    {
        private readonly string _connectionString;

        public CostEntryRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<CostEntry> CreateCostEntry(CostEntry costEntry)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                costEntry.CostEntryId = Guid.NewGuid();
                var cmd = new SqlCommand(
                    @"INSERT INTO CostEntries (CostEntryId, CostCategory, Amount, Date, Description)
                  VALUES (@Id, @Category, @Amount, @Date, @Description)", connection);

                cmd.Parameters.AddWithValue("@Id", costEntry.CostEntryId);
                cmd.Parameters.AddWithValue("@Category", costEntry.CostCategory);
                cmd.Parameters.AddWithValue("@Amount", costEntry.Amount);
                cmd.Parameters.AddWithValue("@Date", costEntry.Date);
                cmd.Parameters.AddWithValue("@Description", costEntry.Description);

                await cmd.ExecuteNonQueryAsync();
                return costEntry;
            }
        }

        public async Task<CostEntry> GetCostEntryById(Guid id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand("SELECT * FROM CostEntries WHERE CostEntryId = @Id", connection);
                cmd.Parameters.AddWithValue("@Id", id);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new CostEntry
                        {
                            CostEntryId = reader.GetGuid(0),
                            CostCategory = reader.GetString(1),
                            Amount = reader.GetDecimal(2),
                            Date = reader.GetDateTime(3),
                            Description = reader.GetString(4)
                        };
                    }
                    return null;
                }
            }
        }

        public async Task<IEnumerable<CostEntry>> GetAllCostEntries()
        {
            var entries = new List<CostEntry>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand("SELECT * FROM CostEntries", connection);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        entries.Add(new CostEntry
                        {
                            CostEntryId = reader.GetGuid(0),
                            CostCategory = reader.GetString(1),
                            Amount = reader.GetDecimal(2),
                            Date = reader.GetDateTime(3),
                            Description = reader.GetString(4)
                        });
                    }
                }
            }
            return entries;
        }

        public async Task<bool> UpdateCostEntry(CostEntry costEntry)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(
                    @"UPDATE CostEntries 
                  SET CostCategory = @Category, 
                      Amount = @Amount, 
                      Date = @Date, 
                      Description = @Description
                  WHERE CostEntryId = @Id", connection);

                cmd.Parameters.AddWithValue("@Id", costEntry.CostEntryId);
                cmd.Parameters.AddWithValue("@Category", costEntry.CostCategory);
                cmd.Parameters.AddWithValue("@Amount", costEntry.Amount);
                cmd.Parameters.AddWithValue("@Date", costEntry.Date);
                cmd.Parameters.AddWithValue("@Description", costEntry.Description);

                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }

        public async Task<bool> DeleteCostEntry(Guid id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand("DELETE FROM CostEntries WHERE CostEntryId = @Id", connection);
                cmd.Parameters.AddWithValue("@Id", id);
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }
    }
}
