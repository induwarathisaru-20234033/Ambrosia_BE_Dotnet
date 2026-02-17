using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMB.Application.Dtos
{
    public class PermissionDto
    {
        public int Id { get; set; }
        public string PermissionCode { get; set; }
        public string Name { get; set; }
        public int FeatureId { get; set; }
        public string FeatureCode { get; set; }
        public string FeatureName { get; set; }
    }
}
