using System.ComponentModel.DataAnnotations;

namespace AMB.Domain.Entities
{
    public class Employee: BaseEntity
    {
        public string EmployeeId { get; set; }

        public string? UserId { get; set; }

        [StringLength(100)]
        public string FirstName { get; set; }

        [StringLength(100)]
        public string LastName { get; set; }

        public string MobileNumber { get; set; }

        [StringLength(100)]
        public string Username { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(250)]
        public string Address { get; set; }

        public List<EmployeeRoleMap>? EmployeeRoleMaps { get; set; }
    }
}
