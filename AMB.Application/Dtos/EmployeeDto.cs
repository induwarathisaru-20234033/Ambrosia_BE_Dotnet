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
        public DateTimeOffset CreatedDate { get; set; }
    }
}
