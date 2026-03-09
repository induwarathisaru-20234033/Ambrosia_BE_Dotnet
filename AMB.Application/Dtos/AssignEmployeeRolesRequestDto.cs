namespace AMB.Application.Dtos
{
    public class AssignEmployeeRolesRequestDto
    {
        public int EmployeeId { get; set; }
        public List<int> RoleIds { get; set; } = new();
        public List<int> CustomRoleIds { get; set; } = new();
    }
}