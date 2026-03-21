namespace AMB.Application.Dtos
{
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNumber { get; set; }
        public string Username { get; set; }
        public string? Email { get; set; }
        public string Address { get; set; }
        public bool IsOnline { get; set; }
        public int Status { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }

    public class WaiterAllocationDto
    {
        public int WaiterId { get; set; }
        public string EmployeeId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public bool IsOnline { get; set; }
        public int AllocatedReservationCount { get; set; }
        public int AllocatedTableCount { get; set; }
        public List<WaiterAllocatedReservationDto> Reservations { get; set; } = new();
        public List<WaiterAllocatedTableDto> Tables { get; set; } = new();
    }

    public class WaiterAllocatedReservationDto
    {
        public int ReservationId { get; set; }
        public string ReservationCode { get; set; } = string.Empty;
        public DateTimeOffset ReservationDate { get; set; }
        public int ReservationStatus { get; set; }
        public int PartySize { get; set; }
        public int TableId { get; set; }
        public string TableName { get; set; } = string.Empty;
    }

    public class WaiterAllocatedTableDto
    {
        public int TableId { get; set; }
        public string TableName { get; set; } = string.Empty;
        public int Capacity { get; set; }
    }
}
