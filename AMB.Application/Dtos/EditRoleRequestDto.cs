namespace AMB.Application.Dtos
{
    public class EditRoleRequestDto
    {
        public int Id { get; set; }
        public string RoleCode { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int Status { get; set; }
        public List<int> PermissionIds { get; set; } = new List<int>();
    }
}