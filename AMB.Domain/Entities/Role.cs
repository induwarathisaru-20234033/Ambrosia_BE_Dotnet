using System.ComponentModel.DataAnnotations;

namespace AMB.Domain.Entities
{
    public class Role: BaseEntity
    {
        [StringLength(10)]
        public string RoleCode { get; set; }

        [StringLength(25)]
        public string RoleName { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        //Navigation Properties
        public List<EmployeeRoleMap>? EmployeeRoleMaps { get; set; }
        public List<RolePermissionMap>? RolePermissionMaps { get; set; }
    }
}
