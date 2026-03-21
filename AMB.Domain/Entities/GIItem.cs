using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMB.Domain.Entities
{
    public class GIItem : BaseEntity
    {
        public int LineItemNo { get; set; }
        public float RequestedQuantity { get; set; }
        public float IssuedQuantity { get; set; }
        public string IssuedFrom { get; set; } = string.Empty;
        public string IssuedTo { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;

        public int InventoryItemId { get; set; }
        public virtual InventoryItem? InventoryItem { get; set; }

        public int GoodIssueNoteId { get; set; }
        public virtual GoodIssueNote? GoodIssueNote { get; set; }
    }
}
