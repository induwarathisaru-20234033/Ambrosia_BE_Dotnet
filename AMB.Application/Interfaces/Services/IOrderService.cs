using AMB.Application.Dtos;
using AMB.Domain.Enums;

namespace AMB.Application.Interfaces.Services
{
    public interface IOrderService
    {
        Task<OrderResponseDto> CreateOrderAsync(CreateOrderRequestDto request);
        Task<OrderResponseDto> GetOrderByIdAsync(int id);
        Task<OrderResponseDto> SendDraftToKdsAsync(SendOrderToKdsDto dto);
        Task<OrderResponseDto> UpdateOrderStatusAsync(UpdateOrderStatusDto dto);
        Task<List<OrderResponseDto>> GetOrdersByStatusAsync(OrderStatus status);
        Task<List<OrderResponseDto>> GetKitchenOrdersAsync();
        Task<PagedResponseDto<OrderResponseDto>> SearchOrdersAsync(SearchOrderRequestDto request);
        Task UpdateDraftOrderAsync(UpdateOrderRequestDto dto);
        Task RemoveItemFromOrderAsync(int orderId, int menuItemId);
    }
}
