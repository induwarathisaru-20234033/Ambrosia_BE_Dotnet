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

        [Required]
        [StringLength(20)]
        public string OrderStatus { get; set; } = "Draft"; // "Draft", "Sent to KDS", "Preparing", "On Hold", "Ready", "Served", "Cancelled".

        public DateTimeOffset? SentToKitchenAt { get; set; }

        // Navigation properties
        public Table? Table { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new();
    }
}