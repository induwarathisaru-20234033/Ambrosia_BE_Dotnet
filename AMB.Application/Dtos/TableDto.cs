using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMB.Application.Dtos
{
    public class TableDto
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public int Capacity { get; set; }
        public bool IsOnlineBookingEnabled { get; set; }
    }
}
