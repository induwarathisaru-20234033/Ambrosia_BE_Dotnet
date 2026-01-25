using System.ComponentModel.DataAnnotations;

namespace AMB.Domain.Entities
{
    public class Permission : BaseEntity
    {
        public int FeatureId { get; set; }

        [StringLength(10)]
        public string PermissionCode { get; set; }

        [StringLength(25)]
        public string PermissionName { get; set; }

        // Navigation property
        public Feature? Feature { get; set; }

        public List<RolePermissionMap>? RolePermissionMaps { get; set; }
    }
}
