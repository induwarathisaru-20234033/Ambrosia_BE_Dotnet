using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMB.Application.Dtos
{
    public class CreateGoodReceiptNoteDto
    {
        public string ReceivedBy { get; set; }
        public DateTimeOffset ReceivedDate { get; set; }
        public string ReceivingFacility { get; set; }
        public int GRNStatus { get; set; }
        public List<CreateGRNItemDto> Items { get; set; }
    }

    public class CreateGRNItemDto
    {
        public int PRItemId { get; set; }
        public int LineItemNo { get; set; }
        public float ReceivedQuantity { get; set; }
        public float AcceptedQuantity { get; set; }
        public float RejectedQuantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string? Remarks { get; set; }
    }
}
