using System.ComponentModel.DataAnnotations;

namespace AMB.Domain.Entities
{
    public class CustomRole : BaseEntity
    {
        public string RoleCode { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Navigation Properties
        public List<CustomRolePermissionMap>? CustomRolePermissionMaps { get; set; }
    }
}