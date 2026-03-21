namespace AMB.Application.Dtos
{
    public class GoodIssueNoteFilterRequestDto : BaseSearchRequestDto
    {
        public string? GINumber { get; set; }
        public string? IssuedBy { get; set; }
        public DateTimeOffset? IssuedDateFrom { get; set; }
        public DateTimeOffset? IssuedDateTo { get; set; }
    }
}