namespace AMB.Application.Dtos
{
    public class PurchaseRequestDto
    {
        public int Id { get; set; }
        public string PurchaseRequestCode { get; set; } = string.Empty;
        public DateTimeOffset CreatedDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Supplier { get; set; } = string.Empty;
        public string RequestedBy { get; set; } = string.Empty;
        public DateTimeOffset RequestedDeliveryDate { get; set; }
        public bool IsUrgent { get; set; }
        public int PurchaseRequestStatus { get; set; }
        public string ReviewedBy { get; set; } = string.Empty;
        public DateTimeOffset ReviewedDate { get; set; }
        public List<PurchaseRequestItemDto> PRItems { get; set; } = new();
    }
}
