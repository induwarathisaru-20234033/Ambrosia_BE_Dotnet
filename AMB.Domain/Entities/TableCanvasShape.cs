using AMB.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMB.Domain.Entities
{
    public class TableCanvasShape : BaseEntity
    {
        public int Type { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Rotation { get; set; }
        public string Fill { get; set; } = "hsl(0, 0%, 90%)";

        public int? AssignedTableId { get; set; }
        public virtual Table? AssignedTable { get; set; }
    }
}
