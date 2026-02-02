namespace AMB.Domain.Entities
{
    public class EmployeeRoleMap: BaseEntity
    {
        public int EmployeeId { get; set; }

        public int RoleId { get; set; }

        // Navigation Properties
        public Employee? Employee { get; set; }

        public Role? Role { get; set; }
    }
}
