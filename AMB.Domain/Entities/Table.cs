using System.ComponentModel.DataAnnotations;

namespace AMB.Domain.Entities
{
    public class Table : BaseEntity
    {
        [StringLength(10)]
        public string TableNumber { get; set; } = string.Empty;

        public int Capacity { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Available"; // "Available", "Occupied", "PendingClean", "Reserved".

        // Navigation properties
        public List<Order> Orders { get; set; } = new();
    }
}
