namespace AMB.Application.Dtos
{
    public class CreateInventoryItemRequestDto
    {
        public string ItemNumber { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public float OpeningQuantity { get; set; }
        public string ItemType { get; set; } = string.Empty;
        public string ItemCategory { get; set; } = string.Empty;
        public string UoM { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        public float MinimumStockLevel { get; set; }
        public float MaximumStockLevel { get; set; }
        public float ReOrderLevel { get; set; }
        public string StorageLocation { get; set; } = string.Empty;
        public float ShelveLife { get; set; }
        public string StorageConditions { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public DateTimeOffset ExpiryDate { get; set; }
    }
}
