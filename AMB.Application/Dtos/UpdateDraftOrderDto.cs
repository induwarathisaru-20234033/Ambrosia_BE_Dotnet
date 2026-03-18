namespace AMB.Application.Dtos
{
    public class UpdateDraftOrderDto
    {
        public int OrderId { get; set; }
        public List<OrderItemDto> Items { get; set; } = new(); // Full list of items (replace existing)
    }
}