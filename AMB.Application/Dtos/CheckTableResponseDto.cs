using System;

namespace AMB.Application.Dtos
{
    public class CheckTableResponseDto
    {
        public bool IsOccupied { get; set; }
        public string? DisplayName { get; set; }
        public string? SessionToken { get; set; } // Temporary "Discovery Token" or session ID
    }
}
