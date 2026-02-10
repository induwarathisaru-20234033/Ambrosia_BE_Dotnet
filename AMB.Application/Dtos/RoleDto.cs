using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMB.Application.Dtos
{
    public class RoleDto
    {
        public int Id { get; set; }
        public string RoleCode { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<PermissionDto> Persmissions { get; set; } = new List<PermissionDto>();
    }

    public class PermissionDto
    {
        public int Id { get; set; }
        public string PermissionCode { get; set; }
        public string Name { get; set; }
        public string Module {  get; set; }
    }
}
