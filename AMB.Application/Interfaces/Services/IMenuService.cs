using AMB.Application.Dtos;

namespace AMB.Application.Interfaces.Services
{
    public interface IMenuService
    {
        Task<MenuItemDto> AddMenuItem(CreateMenuItemDto dto);

        Task<List<MenuItemDto>> GetMenuItems(
            string? category,
            string? name,
            bool? isAvailable
        );
        Task<MenuItemDto> UpdateMenuItem(int id, UpdateMenuItemDto dto);

    }
}