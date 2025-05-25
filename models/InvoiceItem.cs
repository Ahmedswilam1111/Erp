namespace ERPtask.models
{
    public class InvoiceItem
    {
        public Guid ItemId { get; set; }
        public Guid InvoiceId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
