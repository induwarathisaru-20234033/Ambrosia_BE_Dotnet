namespace AMB.Domain.Entities
{
    public class StockTransaction : BaseEntity
    {
        public int InventoryItemId { get; set; }
        public InventoryItem? InventoryItem { get; set; }

        public float Quantity { get; set; }
        public int Direction { get; set; }
        public int TransactionType { get; set; }

        public int ReferenceId { get; set; }
        public int? ReferenceLineId { get; set; }
        public string ReferenceNumber { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        public DateTimeOffset TransactionDate { get; set; }
    }
}