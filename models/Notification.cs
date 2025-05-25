namespace ERPtask.models
{
    public class Notification
    {
        public Guid NotificationId { get; set; }
        public Guid InvoiceId { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public DateTime SentDate { get; set; }
    }
}
