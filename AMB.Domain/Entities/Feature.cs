using System.ComponentModel.DataAnnotations;

namespace AMB.Domain.Entities
{
    public class Feature: BaseEntity
    {
        [StringLength(150)]
        public string FeatureName { get; set; }

        [StringLength(10)]
        public string FeatureCode { get; set; }

        // Navigation property
        public List<Permission>? Permissions { get; set; }
    }
}
