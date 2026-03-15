
using System.ComponentModel.DataAnnotations;

namespace AMB.Domain.Entities
{
    public class OrderItem : BaseEntity
    {
        public int OrderId { get; set; }
        public int MenuItemId { get; set; }

        [StringLength(500)]
        public string? SpecialInstructions { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; } // Price at time of order

        // Navigation properties
        public Order? Order { get; set; }
        //public MenuItem? MenuItem { get; set; }
    }
}
