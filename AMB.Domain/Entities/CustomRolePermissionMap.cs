namespace AMB.Domain.Entities
{
    public class CustomRolePermissionMap : BaseEntity
    {
        public int CustomRoleId { get; set; }
        public int PermissionId { get; set; }

        // Navigation property
        public CustomRole? CustomRole { get; set; }
        public Permission? Permission { get; set; }
    }
}