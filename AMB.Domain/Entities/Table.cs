using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AMB.Domain.Entities
{
    // Table
    [Index(nameof(TableName), IsUnique = true)]
    public class Table: BaseEntity
    {
        // Unique name for the table
        [Required]
        public string TableName { get; set; }

        // Max seating capacity 
        [Range(1, int.MaxValue)]
        public int Capacity { get; set; }

        // Status for customer facing availability
        public bool IsOnlineBookingEnabled { get; set; }
    }
}
