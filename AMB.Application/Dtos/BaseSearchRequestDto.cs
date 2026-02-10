using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMB.Application.Dtos
{
    public class BaseSearchRequestDto
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}
