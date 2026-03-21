using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMB.Domain.Entities
{
    public class GoodIssueNote : BaseEntity
    {
        public string GINumber { get; set; } = string.Empty;
        public string IssuedBy { get; set; } = string.Empty;
        public DateTimeOffset IssuedDate {  get; set; }
        public virtual ICollection<GIItem> GIItems { get; set; } = new List<GIItem>();
    }
}
