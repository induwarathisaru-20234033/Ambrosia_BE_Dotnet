using AMB.Domain.Entities;
using AMB.Domain.Enums;
using AMB.Application.Dtos;

namespace AMB.Application.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(int id, OrderQueryOptions? options = null);
        Task<Order> AddAsync(Order order);
        Task<Order> UpdateAsync(Order order);
        Task<List<Order>> GetDraftOrdersByTableAsync(int tableId);
        Task<string> GenerateOrderNumberAsync();
        Task<bool> SendDraftToKdsAsync(int orderId, int? tableId = null);
        Task<List<Order>> GetOrdersByStatusAsync(OrderStatus status);
        Task<List<Order>> GetKitchenOrdersAsync(); // Gets "Sent to KDS", "Preparing", "On Hold"
        Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus, string? reason = null);
        Task<Order?> GetOrderWithDetailsAsync(int id);
        Task<(List<Order> Items, int TotalCount)> SearchOrdersAsync(SearchOrderRequestDto request);
        Task<bool> UpdateDraftOrderItemsAsync(int orderId, List<OrderItemDto> items);
    }

    public class OrderQueryOptions
    {
        public bool IncludeItems { get; set; }
        public bool IncludeMenuItem { get; set; }
        public bool IncludeTable { get; set; }
    }
}