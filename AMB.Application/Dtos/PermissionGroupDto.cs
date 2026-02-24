using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMB.Application.Dtos
{
    public class PermissionGroupDto
    {
        public int FeatureId {  get; set; }
        public string FeatureCode { get; set; }
        public string FeatureName { get; set; }
        public List<PermissionItemDto> Permissions { get; set; } = new List<PermissionItemDto>();
    }

    public class PermissionItemDto 
    { 
        public int Id { get; set; }
        public string PermissionCode { get; set; }
        public String Name { get; set; }
        public string? Description { get; set; }
        public bool IsSelected { get; set; }
    }
}
