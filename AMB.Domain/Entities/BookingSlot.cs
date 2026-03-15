using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMB.Domain.Entities
{
    public class BookingSlot : BaseEntity
    {
        public Guid SlotId { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int Day { get; set; }

        public int? ShiftId { get; set; }
        public virtual ServiceHour? Shift { get; set; }
    }
}
