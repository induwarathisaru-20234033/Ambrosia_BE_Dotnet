using AMB.Domain.Enums;

namespace AMB.Application.Dtos
{
    public class OrderStatusResponseDto
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public OrderStatus OrderStatus { get; set; }
        public int EstimatedWaitTime { get; set; }
        public bool CanModify { get; set; }
        public bool CanCancel { get; set; }
        public List<OrderItemStatusDto> Items { get; set; } = new();
    }

    public class OrderItemStatusDto
    {
        public int MenuItemId { get; set; }
        public string MenuItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal => Quantity * UnitPrice;
        public string Status { get; set; } = string.Empty;
    }
}