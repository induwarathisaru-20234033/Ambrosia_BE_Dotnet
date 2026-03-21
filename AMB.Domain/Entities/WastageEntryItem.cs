using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMB.Domain.Entities
{
    public class WastageEntryItem : BaseEntity
    {
        public int ItemNo { get; set; }
        public string WastageType { get; set; } = string.Empty;
        public float Quantity { get; set; }
        public string Reason { get; set; } = string.Empty;

        public int InventoryItemId { get; set; }
        public virtual InventoryItem? InventoryItem { get; set; }

        public int WastageRecordId { get; set; }
        public virtual WastageRecord? WastageRecord { get; set; }
    }
}
