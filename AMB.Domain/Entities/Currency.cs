using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMB.Domain.Entities
{
    public class Currency: BaseEntity
    {
        public string CurrencyCode { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
