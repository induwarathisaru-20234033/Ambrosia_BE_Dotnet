using System;

namespace AMB.Application.Dtos
{
    public class CreateCalenderExclusionRequestDto
    {
        public DateTimeOffset ExclusionDate { get; set; }
        public string Reason { get; set; }
    }
}
