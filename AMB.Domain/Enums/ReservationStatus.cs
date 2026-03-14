namespace AMB.Domain.Enums
{
    // Booked is the initial status when a reservation is created.
    public enum ReservationStatus
    {
        Booked = 1,
        Arrived = 2,
        NoShow = 3,
        Cancelled = 4
    }
}
