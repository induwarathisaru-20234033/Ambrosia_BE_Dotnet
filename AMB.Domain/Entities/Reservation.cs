namespace AMB.Domain.Entities
{
    public class Reservation : BaseEntity
    {
        public string ReservationCode { get; set; } = string.Empty;
        public int PartySize { get; set; }
        public int ReservationStatus { get; set; }

        // Actual date of the reservation (includes date + time)
        public DateTimeOffset ReservationDate { get; set; }

        // Optional occasion (e.g., Birthday, Anniversary)
        public string? Occasion { get; set; }

        // Optional special requests from customer
        public string? SpecialRequests { get; set; }

        public DateTimeOffset? ArrivedAt { get; set; }
        public DateTimeOffset? NoShowMarkedAt { get; set; }
        public DateTimeOffset? CancelledAt { get; set; }

        // Foreign key to CustomerDetail
        public int CustomerDetailId { get; set; }
        public virtual CustomerDetail CustomerDetail { get; set; } = null!;

        // Foreign key to BookingSlot (the time slot being reserved)
        public int BookingSlotId { get; set; }
        public virtual BookingSlot BookingSlot { get; set; } = null!;

        // Foreign key to Table (the specific table assigned)
        public int TableId { get; set; }
        public virtual Table Table { get; set; } = null!;

        // Foreign key to Assign Waiter (nullable — null means no waiter assigned)
        public int? AssignedWaiterId { get; set; }
        public virtual Employee? AssignedWaiter { get; set; }

        // All waiter assignment records (active + historical) for this reservation
        public virtual ICollection<ReservationWaiterAssignment> WaiterAssignments { get; set; } = new List<ReservationWaiterAssignment>();
    }
}
