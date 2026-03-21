namespace AMB.Domain.Entities
{
    /// <summary>
    /// Junction entity tracking which waiters are (or have been) assigned to a reservation.
    /// Used by the background auto-assign service to manage waiter workloads.
    /// Status = 1 (Active) means the assignment is currently active.
    /// Status = 0 (Inactive) means the waiter was unassigned (historical record).
    /// </summary>
    public class ReservationWaiterAssignment : BaseEntity
    {
        public int ReservationId { get; set; }
        public virtual Reservation Reservation { get; set; } = null!;

        public int WaiterId { get; set; }
        public virtual Employee Waiter { get; set; } = null!;

        public DateTimeOffset AssignedAt { get; set; }
        public DateTimeOffset? UnassignedAt { get; set; }
    }
}
