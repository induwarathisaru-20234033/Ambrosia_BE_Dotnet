using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AMB.Application.Dtos
{
    public class RoleDto
    {
        public int Id { get; set; }
        public string RoleCode { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int Status { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public List<PermissionDto> Permissions { get; set; } = new List<PermissionDto>();
    }

    
}
