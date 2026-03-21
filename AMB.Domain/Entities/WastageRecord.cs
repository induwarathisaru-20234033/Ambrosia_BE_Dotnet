using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMB.Domain.Entities
{
    public class WastageRecord : BaseEntity
    {
        public string WastageEntryNumber { get; set; }  = string.Empty;
        public DateTimeOffset EntryDate { get; set; }
        public string RecordedBy { get; set; } = string.Empty;
        public string GeneralNotes { get; set; } = string.Empty;

        public virtual ICollection<WastageEntryItem> WastageEntryItems { get; set; } = new List<WastageEntryItem>();
    }
}
