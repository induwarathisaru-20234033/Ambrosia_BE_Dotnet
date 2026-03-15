using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMB.Domain.Entities
{
    public class InventoryItem : BaseEntity
    {
        // Unique internal identifier or code for the item (e.g., PART-101).
        public string ItemNumber { get; set; } = string.Empty;

        // The full display name of the inventory item.
        public string ItemName { get; set; } = string.Empty ;

        // The initial stock count when the item was first recorded in the system.
        public float OpeningQuantity { get; set; }

        // The remaining stock count
        public float RemainingQuantity { get; set; }

        // The classification of the item (e.g., Raw Material, Finished Good, Service)
        public string ItemType { get; set; } = string.Empty;

        //Broad grouping for the item (e.g., Electronics, Hardware, Perishables)
        public string ItemCategory {  get; set; } = string.Empty;

        // Unit of Measure (e.g., KG, Liters, Boxes, Pieces).
        public string UoM { get; set; } = string.Empty;

        // The cost or selling price per single unit.
        public decimal UnitPrice { get; set; }

        // The ISO currency code associated with the price
        public string Currency { get; set; } = string.Empty;

        // Additional notes, descriptions, or internal observations about the item
        public string Remarks { get; set; } = string.Empty;

        // The safety stock level. Alerts should trigger if stock falls below this point.
        public float MinimumStockLevel { get; set; }

        // The upper limit of stock to be held to prevent overstocking and high holding costs.
        public float MaximumStockLevel { get; set; }

        // The specific inventory count that triggers a new purchase order
        public float ReOrderLevel { get; set; }

        //Physical location in the warehouse(e.g., Aisle 4, Shelf B, Bin 12)
        public string StorageLocation {  get; set; } = string.Empty;

        // The duration (often in days) the item remains usable before expiring.
        public float ShelveLife { get; set; }

        // Specific environment needs (e.g., "Store below 20°C", "Keep away from moisture").
        public string StorageConditions {  get; set; } = string.Empty;

        // Stock Keeping Unit; the barcode or external scan code for the item.
        public string Sku {  get; set; } = string.Empty;

        // The date and time when the current batch of items will expire.
        public DateTimeOffset ExpiryDate { get; set; }

        // Numeric representation of the item's state (Low, High, Normal)
        public int InventoryStatus { get; set; }
    }
}
