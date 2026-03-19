using AMB.Domain.Enums;

namespace AMB.Application.Dtos
{
    public class OrderResponseDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public int? TableId { get; set; }
        public string? TableName { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset? UpdatedDate { get; set; }
        public List<OrderItemResponseDto> Items { get; set; } = new();
    }

    public class OrderItemResponseDto
    {
        public int Id { get; set; }
        public int MenuItemId { get; set; }
        public string MenuItemName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? SpecialInstructions { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal => Quantity * UnitPrice;
        public bool IsAvailable { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public OrderStatus? ItemStatus { get; set; }
    }
}