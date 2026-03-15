using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace AMB.Domain.Entities
{
    public class Order : BaseEntity
    {
        [StringLength(20)]
        public string OrderNumber { get; set; } = string.Empty;

        public int? TableId { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Draft"; // "Draft", "Sent to KDS", "Preparing", "On Hold", "Ready", "Served", "Cancelled".

        public DateTimeOffset? SentToKitchenAt { get; set; }

        // Navigation properties
        public Table? Table { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new();
    }
}
