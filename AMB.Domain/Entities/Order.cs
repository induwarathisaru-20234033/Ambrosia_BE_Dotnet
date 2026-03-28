using System.ComponentModel.DataAnnotations;
using AMB.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AMB.Domain.Entities
{
    public class Order : BaseEntity
    {
        [Required]
        [StringLength(20)]
        public string OrderNumber { get; set; } = string.Empty;

        public int? TableId { get; set; }

        public int? ReservationId { get; set; }

        public int OrderStatus { get; set; } = (int)AMB.Domain.Enums.OrderStatus.Draft;

        public DateTimeOffset? SentToKitchenAt { get; set; }

        // Navigation properties
        public Table? Table { get; set; }
        public Reservation? Reservation { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new();
    }
}