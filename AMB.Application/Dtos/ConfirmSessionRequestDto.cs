using System;

namespace AMB.Application.Dtos
{
    public class ConfirmSessionRequestDto
    {
        public Guid TableCode { get; set; }
        public string? ReservationCode { get; set; } // Required if ConfirmationType is "Manual"
        public string ConfirmationType { get; set; } = string.Empty; // "Implicit" or "Manual"
    }
}
