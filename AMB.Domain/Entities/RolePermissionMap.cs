namespace AMB.Domain.Entities
{
    public class RolePermissionMap: BaseEntity
    {
        public int RoleId { get; set; }
        public int PermissionId { get; set; }

        // Navigation property
        public Role? Role { get; set; }
        public Permission? Permission { get; set; }
    }
}
