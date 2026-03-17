namespace AMB.Application.Dtos
{
    public class PurchaseRequestFilterRequestDto : BaseSearchRequestDto
    {
        public string? PurchaseRequestCode { get; set; }
        public string? Supplier { get; set; }
        public string? RequestedBy { get; set; }
        public int? PurchaseRequestStatus { get; set; }
        public DateTimeOffset? CreatedDateFrom { get; set; }
        public DateTimeOffset? CreatedDateTo { get; set; }
    }
}
