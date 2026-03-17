using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMB.Domain.Entities
{
    public class PurchaseRequest : BaseEntity
    {
        public string PurchaseRequestCode { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Supplier { get; set; } = string.Empty;
        public string RequestedBy { get; set; } = string.Empty;
        public DateTimeOffset RequestedDeliveryDate { get; set; }
        public bool IsUrgent { get; set; }
        public int PurchaseRequestStatus { get; set; }
        public virtual ICollection<PurchaseRequestItem> PRItems { get; set; } = new List<PurchaseRequestItem>();
        public string ReviewedBy { get; set; } = string.Empty;
        public DateTimeOffset ReviewedDate { get; set; }
    }
}
