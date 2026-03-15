namespace AMB.Application.Dtos
{
    public class CreatePurchaseRequestDto
    {
        public string Description { get; set; } = string.Empty;
        public string Supplier { get; set; } = string.Empty;
        public string RequestedBy { get; set; } = string.Empty;
        public DateTimeOffset RequestedDeliveryDate { get; set; }
        public bool IsUrgent { get; set; }
        public List<CreatePurchaseRequestItemDto> PRItems { get; set; } = new();
    }

    public class CreatePurchaseRequestItemDto
    {
        public int LineItemNo { get; set; }
        public float RequestedQuantity { get; set; }
        public float Price { get; set; }
        public int InventoryItemId { get; set; }
    }
}
