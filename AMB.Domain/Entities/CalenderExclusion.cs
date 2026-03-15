namespace AMB.Domain.Entities
{
    // Closing dates of the restaurant
    public class CalenderExclusion: BaseEntity
    {
        public DateTimeOffset ExclusionDate { get; set; }

        public string Reason { get; set; }
    }
}
