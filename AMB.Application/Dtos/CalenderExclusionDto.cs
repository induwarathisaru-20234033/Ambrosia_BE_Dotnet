using System;

namespace AMB.Application.Dtos
{
    public class CalenderExclusionDto
    {
        public int Id { get; set; }
        public DateTimeOffset ExclusionDate { get; set; }
        public string Reason { get; set; }
    }
}
