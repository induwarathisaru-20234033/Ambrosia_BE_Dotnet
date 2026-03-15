using AMB.Application.Dtos;

namespace AMB.Application.Interfaces.Services
{
    public interface IOrderService
    {
        Task<OrderResponseDto> CreateOrderAsync(CreateOrderRequestDto request);
        Task<OrderResponseDto> GetOrderByIdAsync(int id);
        Task<OrderResponseDto> SendDraftToKdsAsync(SendOrderToKdsDto dto);
        Task<OrderResponseDto> UpdateOrderStatusAsync(UpdateOrderStatusDto dto);
        Task<List<OrderResponseDto>> GetOrdersByStatusAsync(string status);
        Task<List<OrderResponseDto>> GetKitchenOrdersAsync();
    }
}
