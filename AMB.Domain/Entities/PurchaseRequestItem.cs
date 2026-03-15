using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMB.Domain.Entities
{
    public class PurchaseRequestItem : BaseEntity
    {
        public int LineItemNo { get; set; }
        public float RequestedQuantity { get; set; }
        public float Price { get; set; }

        public int PurchaseRequestId { get; set; }
        public virtual PurchaseRequest PurchaseRequest { get; set; } = null!;

        public int InventoryItemId { get; set; }
        public virtual InventoryItem InventoryItem { get; set; } = null!;
    }
}
