namespace AMB.Application.Dtos
{
    public class PurchaseRequestItemDto
    {
        public int Id { get; set; }
        public int LineItemNo { get; set; }
        public float RequestedQuantity { get; set; }
        public decimal Price { get; set; }
        public int InventoryItemId { get; set; }
    }
}
