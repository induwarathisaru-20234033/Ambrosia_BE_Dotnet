using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMB.Domain.Entities
{
    public class GoodReceiptNote : BaseEntity
    {
        public string GRNNumber { get; set; } = string.Empty;
        public string Supplier { get; set; } = string.Empty;
        public DateTimeOffset ReceivedDate { get; set; }
        public string ReceivedBy { get; set; } = string.Empty;
        public string ReceivedFacility { get; set; } = string.Empty;
        
        public virtual ICollection<GRNItem> GRNItems { get; set; } = new List<GRNItem>();

        public int PurchaseRequestId { get; set; }
        public virtual PurchaseRequest? PurchaseRequest { get; set; }

    }
}
