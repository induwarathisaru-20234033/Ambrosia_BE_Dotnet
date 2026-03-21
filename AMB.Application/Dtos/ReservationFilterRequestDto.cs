using System;

namespace AMB.Application.Dtos
{
    public class ReservationFilterRequestDto : BaseSearchRequestDto
    {
        public string? ReservationCode { get; set; }
        public int? ReservationStatus { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerPhone { get; set; }
        public string? Table { get; set; }
        public DateTimeOffset? ReservationDateFrom { get; set; }
        public DateTimeOffset? ReservationDateTo { get; set; }
        public DateTimeOffset? CreatedDateFrom { get; set; }
        public DateTimeOffset? CreatedDateTo { get; set; }
    }
}
