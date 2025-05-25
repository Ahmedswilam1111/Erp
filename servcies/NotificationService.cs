using ERPtask.models;
using ERPtask.Repositrories;
using System.Data.SqlClient;
using System.Net.Mail;
namespace ERPtask.servcies
{
    public class NotificationService
    {
        private readonly NotificationRepository _repository;
        private readonly InvoiceRepository _invoiceRepo;
        private readonly ClientRepository _clientRepo;

        public NotificationService(
            NotificationRepository repository,
            InvoiceRepository invoiceRepo,
            ClientRepository clientRepo)
        {
            _repository = repository;
            _invoiceRepo = invoiceRepo;
            _clientRepo = clientRepo;
        }

        public async Task<Notification> SendDueReminder(Guid invoiceId, string type)
        {
            var invoice = await _invoiceRepo.GetInvoiceById(invoiceId);
            if (invoice == null)
                throw new ArgumentException("Invoice not found");

            var client = await _clientRepo.GetClientById(invoice.ClientId);
            if (client == null)
                throw new ArgumentException("Client not found");

            var notification = new Notification
            {
                InvoiceId = invoiceId,
                Type = type,
                Status = "Pending",
                SentDate = DateTime.UtcNow
            };

            try
            {
                // Simulate actual notification sending
                await SendActualNotification(client, invoice, type);
                notification.Status = "Sent";
            }
            catch
            {
                notification.Status = "Failed";
            }

            return await _repository.CreateNotification(notification);
        }

        public async Task<IEnumerable<Notification>> GetInvoiceNotifications(Guid invoiceId)
        {
            return await _repository.GetNotificationsByInvoice(invoiceId);
        }

        public async Task<bool> RetryFailedNotification(Guid notificationId)
        {
            var notification = await _repository.GetNotificationById(notificationId);
            if (notification?.Status != "Failed") return false;

            var invoice = await _invoiceRepo.GetInvoiceById(notification.InvoiceId);
            var client = await _clientRepo.GetClientById(invoice.ClientId);

            try
            {
                await SendActualNotification(client, invoice, notification.Type);
                return await _repository.UpdateNotificationStatus(notificationId, "Sent");
            }
            catch
            {
                return false;
            }
        }
        public async Task<Notification> GetNotificationById(Guid id)
        {
            return await _repository.GetNotificationById(id);
        }

        public async Task<bool> DeleteNotification(Guid id)
        {
            // Check if notification exists first
            var notification = await _repository.GetNotificationById(id);
            if (notification == null) return false;

            
            if (notification.Status == "Sent")
            {
                throw new InvalidOperationException("Cannot delete successfully sent notifications");
            }

            return await _repository.DeleteNotification(id);
        }
        private async Task SendActualNotification(Client client, Invoice invoice, string type)
        {
            // Implementation would vary based on notification channel
            // This is a simulation that always succeeds
            await Task.Delay(100); // Simulate network call

            // Uncomment to test failure scenarios
            // throw new Exception("Notification service unavailable");
        }
    }

}
