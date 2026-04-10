namespace AMB.Application.Dtos
{
    public class AssignRoleRequestDto
    {
        public int RoleId { get; set; }
        public List<int> EmployeeIds { get; set; } = new();
    }
}