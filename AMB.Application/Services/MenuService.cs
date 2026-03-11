using AMB.Application.Dtos;
using AMB.Application.Interfaces;

namespace AMB.Application.Services
{
    public class MenuService : IMenuService
    {
        private static List<MenuItemDto> menuItems = new();

        public async Task AddMenuItem(MenuItemDto menuItemDto)
        {
            menuItems.Add(menuItemDto);
        }

        public async Task<List<MenuItemDto>> GetMenuItems()
        {
            return menuItems;
        }
    }
}