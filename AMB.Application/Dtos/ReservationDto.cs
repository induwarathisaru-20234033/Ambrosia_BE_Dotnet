using System;

namespace AMB.Application.Dtos
{
    public class CreateReservationRequestDto
    {
        public int PartySize { get; set; }
        public DateTimeOffset ReservationDate { get; set; }
        public string? Occasion { get; set; }
        public string? SpecialRequests { get; set; }
        
        // Customer details
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhoneNumber { get; set; } = string.Empty;
        
        // Slot and table selection
        public int BookingSlotId { get; set; }
        public int TableId { get; set; }
    }

    public class ReservationDto
    {
        public int Id { get; set; }
        public string ReservationCode { get; set; } = string.Empty;
        public int PartySize { get; set; }
        public int ReservationStatus { get; set; }
        public DateTimeOffset ReservationDate { get; set; }
        public string? Occasion { get; set; }
        public string? SpecialRequests { get; set; }
        public DateTimeOffset? ArrivedAt { get; set; }
        public DateTimeOffset? NoShowMarkedAt { get; set; }
        public DateTimeOffset? CancelledAt { get; set; }
        
        public CustomerDetailDto? CustomerDetail { get; set; }
        public BookingSlotDto? BookingSlot { get; set; }
        public TableDto? Table { get; set; }
    }

    public class CustomerDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }

    public class BookingSlotDto
    {
        public int Id { get; set; }
        public Guid SlotId { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int Day { get; set; }
        public int ExistingAllocations { get; set; }
    }

    public class ReservationAvailabilityDto
    {
        public int BookingSlotId { get; set; }
        public int TableId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool IsSlotAvailable { get; set; }
        public bool IsTableAvailable { get; set; }
        public int ExistingReservationsForSlot { get; set; }
        public int ExistingReservationsForTable { get; set; }
    }
}
