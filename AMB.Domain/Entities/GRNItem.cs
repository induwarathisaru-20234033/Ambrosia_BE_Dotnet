using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMB.Domain.Entities
{
    public class GRNItem : BaseEntity
    {
        public int LineItemNo { get; set; }
        public float ReceivedQuantity { get; set; }
        public float AcceptedQuantity { get; set; }
        public float RejectedQuantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string Remarks { get; set; } = string.Empty;

        public int PRItemId { get; set; }
        public virtual PurchaseRequestItem? PRItem { get; set; }

        public int GRNId { get; set; }
        public GoodReceiptNote? GoodReceiptNote { get; set; }
    }
}
