namespace ERPtask.models
{
    public class CostEntry
    {
        public Guid CostEntryId { get; set; }
        public string CostCategory { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }
}
