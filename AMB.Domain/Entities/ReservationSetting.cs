namespace AMB.Domain.Entities
{
    public class ReservationSetting: BaseEntity
    {
        // Avarage dining duration in minutes
        public int TurnTime { get; set; }

        // Cleaning gap in minutes
        public int BufferTime { get; set; }

        // Frequency of slots in customer portal
        public int BookingInterval { get; set; }
    }
}
