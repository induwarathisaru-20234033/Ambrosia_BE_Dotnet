namespace AMB.Application.Dtos
{
    public class RoleDetailDto
    {
        public int Id { get; set; }
        public string RoleCode { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int Status { get; set; }
        public List<int> SelectedPermissionIds { get; set; } = new List<int>();
        public List<PermissionGroupDto> PermissionGroups { get; set; } = new();
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset? UpdatedDate { get; set; }
    }
}