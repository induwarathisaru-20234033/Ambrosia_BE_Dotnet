using AMB.Application.Dtos;

namespace AMB.Application.Interfaces.Services
{
    public interface IOrderService
    {
        Task<OrderResponseDto> CreateOrderAsync(CreateOrderRequestDto request);
        Task<OrderResponseDto> GetOrderByIdAsync(int id);
        Task<List<MenuItemDto>> SearchMenuItemsAsync(string searchTerm);
    }
}
