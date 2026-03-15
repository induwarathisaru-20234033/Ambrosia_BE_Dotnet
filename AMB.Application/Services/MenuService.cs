using AMB.Application.Dtos;
using AMB.Application.Interfaces;
using AMB.Domain.Entities;
using AMB.Infra.DBContexts;
using Microsoft.EntityFrameworkCore;

namespace AMB.Application.Services
{
    public class MenuService : IMenuService
    {
        private readonly AMBContext _context;

        public MenuService(AMBContext context)
        {
            _context = context;
        }

        public async Task AddMenuItem(MenuItemDto menuItemDto)
        {
            var menuItem = new MenuItem
            {
                Name = menuItemDto.Name,
                Price = menuItemDto.Price,
                Category = menuItemDto.Category,
                IsAvailable = menuItemDto.IsAvailable
            };

            _context.MenuItems.Add(menuItem);
            await _context.SaveChangesAsync();
        }

        public async Task<List<MenuItemDto>> GetMenuItems(
            string? category,
            string? name,
            bool? isAvailable)
        {
            var query = _context.MenuItems.AsQueryable();

            if (!string.IsNullOrEmpty(category))
                query = query.Where(x => x.Category == category);

            if (!string.IsNullOrEmpty(name))
                query = query.Where(x => x.Name.ToLower().Contains(name.ToLower()));

            if (isAvailable.HasValue)
                query = query.Where(x => x.IsAvailable == isAvailable.Value);

            var items = await query.ToListAsync();

            return items.Select(x => new MenuItemDto
            {
                Name = x.Name,
                Price = x.Price,
                Category = x.Category,
                IsAvailable = x.IsAvailable
            }).ToList();
        }
    }
}