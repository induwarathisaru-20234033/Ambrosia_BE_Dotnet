namespace AMB.Application.Dtos
{
    public class SendOrderToKdsDto
    {
        public int OrderId { get; set; }
        public int? TableId { get; set; } // In case table was changed
    }
}