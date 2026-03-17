namespace AMB.Application.Dtos
{
    public class GoodReceiptNoteFilterRequestDto : BaseSearchRequestDto
    {
        public string? GRNNumber { get; set; }
        public string? Supplier { get; set; }
        public string? ReceivedBy { get; set; }
        public int? PurchaseRequestId { get; set; }
        public DateTimeOffset? ReceivedDateFrom { get; set; }
        public DateTimeOffset? ReceivedDateTo { get; set; }
    }
}