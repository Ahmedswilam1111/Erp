namespace ERPtask.models
{
    public class Invoice
    {
        public Guid InvoiceId { get; set; }
        public Guid ClientId { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Discounts { get; set; }
        public decimal Total { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }//('Draft', 'Sent', 'Paid', 'Overdue')
        public List<InvoiceItem> Items { get; set; }
    }
}
