using ERPtask.models;
using System.Data.SqlClient;

namespace ERPtask.Repositrories
{
    public class NotificationRepository
    {
        private readonly string _connectionString;

        public NotificationRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<Notification> CreateNotification(Notification notification)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                notification.NotificationId = Guid.NewGuid();
                var cmd = new SqlCommand(
                    @"INSERT INTO Notifications 
                (NotificationId, InvoiceId, Type, Status, SentDate)
                VALUES (@Id, @InvoiceId, @Type, @Status, @SentDate)",
                    connection);

                cmd.Parameters.AddWithValue("@Id", notification.NotificationId);
                cmd.Parameters.AddWithValue("@InvoiceId", notification.InvoiceId);
                cmd.Parameters.AddWithValue("@Type", notification.Type);
                cmd.Parameters.AddWithValue("@Status", notification.Status);
                cmd.Parameters.AddWithValue("@SentDate", notification.SentDate);

                await cmd.ExecuteNonQueryAsync();
                return notification;
            }
        }

        public async Task<Notification> GetNotificationById(Guid id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(
                    "SELECT * FROM Notifications WHERE NotificationId = @Id",
                    connection);
                cmd.Parameters.AddWithValue("@Id", id);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new Notification
                        {
                            NotificationId = reader.GetGuid(0),
                            InvoiceId = reader.GetGuid(1),
                            Type = reader.GetString(2),
                            Status = reader.GetString(3),
                            SentDate = reader.GetDateTime(4)
                        };
                    }
                    return null;
                }
            }
        }


        public async Task<IEnumerable<Notification>> GetNotificationsByInvoice(Guid invoiceId)
        {
            var notifications = new List<Notification>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(
                    "SELECT * FROM Notifications WHERE InvoiceId = @InvoiceId",
                    connection);
                cmd.Parameters.AddWithValue("@InvoiceId", invoiceId);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        notifications.Add(new Notification
                        {
                            NotificationId = reader.GetGuid(0),
                            InvoiceId = reader.GetGuid(1),
                            Type = reader.GetString(2),
                            Status = reader.GetString(3),
                            SentDate = reader.GetDateTime(4)
                        });
                    }
                }
            }
            return notifications;
        }

        public async Task<bool> UpdateNotificationStatus(Guid notificationId, string status)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(
                    "UPDATE Notifications SET Status = @Status WHERE NotificationId = @Id",
                    connection);

                cmd.Parameters.AddWithValue("@Id", notificationId);
                cmd.Parameters.AddWithValue("@Status", status);

                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }

        public async Task<bool> DeleteNotification(Guid notificationId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(
                    "DELETE FROM Notifications WHERE NotificationId = @Id",
                    connection);

                cmd.Parameters.AddWithValue("@Id", notificationId);
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }

    }
}
