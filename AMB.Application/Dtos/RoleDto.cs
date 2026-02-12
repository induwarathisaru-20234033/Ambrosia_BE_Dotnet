using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AMB.Application.Dtos
{
    public class RoleDto
    {
        public int Id { get; set; }
        [StringLength(50)]
        public string RoleCode { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(500)]
        public string? Description { get; set; }
        public string Status { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public List<PermissionDto> Permissions { get; set; } = new List<PermissionDto>();
    }

    
}
