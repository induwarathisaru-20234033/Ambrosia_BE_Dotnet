namespace AMB.Application.Dtos
{
    public class AssignedRoleEmployeeDto
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}".Trim();
        public string Username { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string MobileNumber { get; set; } = string.Empty;
        public bool IsOnline { get; set; }
        public int Status { get; set; }
    }

    public class RoleAssignedEmployeesDto
    {
        public int RoleId { get; set; }
        public string RoleCode { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public bool IsCustomRole { get; set; }
        public List<AssignedRoleEmployeeDto> AssignedEmployees { get; set; } = new();
    }
}