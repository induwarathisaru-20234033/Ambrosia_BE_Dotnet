using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AMB.Domain.Entities
{
    public class Order : BaseEntity
    {
        [Required]
        [StringLength(20)]
        public string OrderNumber { get; set; } = string.Empty;

        public int? TableId { get; set; }

        public int OrderStatus { get; set; } = (int)OrderStatus.Draft;

        public DateTimeOffset? SentToKitchenAt { get; set; }

        // Navigation properties
        public Table? Table { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new();
    }
}