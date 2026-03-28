namespace AMB.Application.Dtos
{
    public class CreateMobileOrderRequestDto
    {
        public int TableId { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
        public bool IsDraft { get; set; } = false;
    }
}