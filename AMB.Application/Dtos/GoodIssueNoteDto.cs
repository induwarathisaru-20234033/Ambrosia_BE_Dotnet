namespace AMB.Application.Dtos
{
    public class GoodIssueNoteDto
    {
        public int Id { get; set; }
        public string GINumber { get; set; } = string.Empty;
        public string IssuedBy { get; set; } = string.Empty;
        public DateTimeOffset IssuedDate { get; set; }
        public List<GIItemDto> Items { get; set; } = new List<GIItemDto>();
    }

    public class GIItemDto
    {
        public int Id { get; set; }
        public int LineItemNo { get; set; }
        public int InventoryItemId { get; set; }
        public InventoryItemDto? InventoryItem { get; set; }
        public float RequestedQuantity { get; set; }
        public float IssuedQuantity { get; set; }
        public string IssuedFrom { get; set; } = string.Empty;
        public string IssuedTo { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
    }
}