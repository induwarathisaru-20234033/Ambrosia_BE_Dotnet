using AMB.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace AMB.Application.Dtos
{
    public class UpdateOrderStatusDto
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        public OrderStatus Status { get; set; } // "Preparing", "On Hold", "Ready", "Served", "Cancelled"

        public string? Reason { get; set; } // Required for "On Hold" and "Cancelled"
    }
}