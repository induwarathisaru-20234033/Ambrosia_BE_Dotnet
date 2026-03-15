namespace AMB.Application.Dtos
{
    public class CreateOrderRequestDto
    {
        public int? TableId { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
        public bool IsDraft { get; set; } // true = Draft, false = Fire
    }

    public class OrderItemDto
    {
        public int MenuItemId { get; set; }
        public string? SpecialInstructions { get; set; }
        public int Quantity { get; set; } = 1;
    }
}