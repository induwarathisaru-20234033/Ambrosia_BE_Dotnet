namespace AMB.Application.Dtos
{
    public class CreateGoodIssueNoteDto
    {
        public string IssuedBy { get; set; } = string.Empty;
        public DateTimeOffset IssuedDate { get; set; }
        public List<CreateGIItemDto> Items { get; set; } = new List<CreateGIItemDto>();
    }

    public class CreateGIItemDto
    {
        public int LineItemNo { get; set; }
        public int InventoryItemId { get; set; }
        public float RequestedQuantity { get; set; }
        public float IssuedQuantity { get; set; }
        public string IssuedFrom { get; set; } = string.Empty;
        public string IssuedTo { get; set; } = string.Empty;
        public string? Remarks { get; set; }
    }
}