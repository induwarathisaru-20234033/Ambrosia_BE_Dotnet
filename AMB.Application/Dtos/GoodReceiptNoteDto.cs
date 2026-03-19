namespace AMB.Application.Dtos
{
    public class GoodReceiptNoteDto
    {
        public int Id { get; set; }
        public string GRNNumber { get; set; } = string.Empty;
        public string Supplier { get; set; } = string.Empty;
        public DateTimeOffset ReceivedDate { get; set; }
        public string ReceivedBy { get; set; } = string.Empty;
        public string ReceivedFacility { get; set; } = string.Empty;
        public int PurchaseRequestId { get; set; }
        public int GRNStatus { get; set; }
        public List<GRNItemDto> Items { get; set; } = new List<GRNItemDto>();
    }

    public class GRNItemDto
    {
        public int Id { get; set; }
        public int LineItemNo { get; set; }
        public int PRItemId { get; set; }
        public PurchaseRequestItemDto? PurchaseRequestItem { get; set; }
        public float ReceivedQuantity { get; set; }
        public float AcceptedQuantity { get; set; }
        public float RejectedQuantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
