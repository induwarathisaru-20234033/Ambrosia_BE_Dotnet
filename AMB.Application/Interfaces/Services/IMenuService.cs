using AMB.Application.Dtos;

namespace AMB.Application.Interfaces.Services
{
    public interface IMenuService
    {
        Task AddMenuItem(MenuItemDto menuItemDto);

        Task<List<MenuItemDto>> GetMenuItems(
            string? category,
            string? name,
            bool? isAvailable
        );
    }
}