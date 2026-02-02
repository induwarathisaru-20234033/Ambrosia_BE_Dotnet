using System.ComponentModel.DataAnnotations;

namespace AMB.Domain.Entities
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [ConcurrencyCheck]
        public int Status { get; set; }

        public DateTimeOffset CreatedDate { get; set; }

        public DateTimeOffset? UpdatedDate { get; set; }

        public DateTimeOffset? DeletedDate { get; set; }

        public string CreatedBy { get; set; }

        public string? UpdatedBy { get; set; }

        public string? DeletedBy { get; set; }
    }
}
