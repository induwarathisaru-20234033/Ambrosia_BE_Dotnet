namespace AMB.Application.Dtos
{
    public class CreateEmployeeRequestDto
    {
        public string EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Email { get; set; }
        public string Username { get; set; }
        public string MobileNumber { get; set; }
        public string Address { get; set; }
    }
}
