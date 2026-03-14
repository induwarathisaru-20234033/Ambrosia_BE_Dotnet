using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMB.Domain.Entities
{
    public class UnitOfMeasure : BaseEntity
    {
        public string UoM { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
