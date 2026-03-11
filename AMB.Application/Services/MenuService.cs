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

        public async Task<List<MenuItemDto>> GetMenuItems(
            string? category,
            string? name,
            bool? isAvailable)
        {
            var query = menuItems.AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(x => x.Category == category);
            }

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(x => x.Name.ToLower().Contains(name.ToLower()));
            }

            if (isAvailable.HasValue)
            {
                query = query.Where(x => x.IsAvailable == isAvailable.Value);
            }

            return query.ToList();
        }
    }
}