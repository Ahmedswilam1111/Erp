using ERPtask.models;
using System.Data.SqlClient;

namespace ERPtask.Repositrories
{
    public class ClientRepository
    {
        private readonly string _connectionString;

        public ClientRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<Client>> GetAllClients()
        {
            var clients = new List<Client>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand("SELECT * FROM Clients", connection);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        clients.Add(new Client
                        {
                            ClientId = reader.GetGuid(0),
                            Email = reader.GetString(1),
                            Phone = reader.GetString(2),
                            Address = reader.GetString(3)
                        });
                    }
                }
            }
            return clients;
        }

        public async Task<Client> GetClientById(Guid clientId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand("SELECT * FROM Clients WHERE ClientId = @ClientId", connection);
                cmd.Parameters.AddWithValue("@ClientId", clientId);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new Client
                        {
                            ClientId = reader.GetGuid(0),
                            Email = reader.GetString(1),
                            Phone = reader.GetString(2),
                            Address = reader.GetString(3)
                        };
                    }
                    return null;
                }
            }
        }

        public async Task<Client> CreateClient(Client client)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                client.ClientId = Guid.NewGuid();
                var cmd = new SqlCommand(
                    @"INSERT INTO Clients (Email, Phone, Address)
                VALUES ( @Email, @Phone, @Address)", connection);

                //cmd.Parameters.AddWithValue("@ClientId", client.ClientId);
                cmd.Parameters.AddWithValue("@Email", client.Email);
                cmd.Parameters.AddWithValue("@Phone", client.Phone);
                cmd.Parameters.AddWithValue("@Address", client.Address);

                await cmd.ExecuteNonQueryAsync();
                return client;
            }
        }

        public async Task<bool> UpdateClient(Client client)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(
                    @"UPDATE Clients 
                SET Email = @Email,
                    Phone = @Phone,
                    Address = @Address
                WHERE ClientId = @ClientId", connection);

                cmd.Parameters.AddWithValue("@ClientId", client.ClientId);
                cmd.Parameters.AddWithValue("@Email", client.Email);
                cmd.Parameters.AddWithValue("@Phone", client.Phone);
                cmd.Parameters.AddWithValue("@Address", client.Address);

                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }

        public async Task<bool> DeleteClient(Guid clientId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(
                    "DELETE FROM Clients WHERE ClientId = @ClientId",
                    connection);

                cmd.Parameters.AddWithValue("@ClientId", clientId);
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }
    }
}
