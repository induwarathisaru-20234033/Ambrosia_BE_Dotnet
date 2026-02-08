using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMB.Application.Dtos
{
    public class CreateRoleRequestDto
    {
        [Required(ErrorMessage ="Role code is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Role ode must be 3-50 characters")]
        [RegularExpression(@"^[A-Z0-9_]+$", ErrorMessage = "Only uppercase letters, numbers, and underscores allowed")]
        public string RoleId { get; set; }

        [Required(ErrorMessage = "Role Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Role Name must be 2-100 characters")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [Required]
        public string Status { get; set; }

        [Required(ErrorMessage = "Please select at least one permission for this role")]
        public List<int> PersmissionIds { get; set; } = new List<int>();

    }
}
