namespace AMB.Domain.Entities
{
    // Standard opening hours of the restaurant
    public class ServiceHour: BaseEntity
    {
        // Day of week (Monday through Sunday)
        public int Day { get; set; }

        // State of the day
        public Boolean IsOpen { get; set; }

        // Start of the service shift
        public TimeOnly StartTime { get; set; }

        // End of the service shift
        public TimeOnly EndTime { get; set; }
    }
}
