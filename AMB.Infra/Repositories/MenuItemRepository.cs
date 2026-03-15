using AMB.Application.Interfaces.Repositories;
using AMB.Domain.Entities;
using AMB.Infra.DBContexts;
using Microsoft.EntityFrameworkCore;

namespace AMB.Infra.Repositories
{
    public class MenuItemRepository : IMenuItemRepository
    {
        private readonly AMBContext _context;

        public MenuItemRepository(AMBContext context)
        {
            _context = context;
        }

        public async Task<MenuItem?> GetByIdAsync(int id)
        {
            return await _context.MenuItems
                .FirstOrDefaultAsync(m => m.Id == id); 
        }

        public async Task<List<MenuItem>> GetByIdsAsync(List<int> ids)
        {
            return await _context.MenuItems
                .Where(m => ids.Contains(m.Id)) 
                .ToListAsync();
        }

        public async Task<List<MenuItem>> GetByCategoryAsync(string category)
        {
            return await _context.MenuItems
                .Where(m => m.Category == category && m.IsAvailable) 
                .OrderBy(m => m.Name)
                .ToListAsync();
        }

        public IQueryable<MenuItem> GetQuery()
        {
            return _context.MenuItems
                .AsQueryable(); 
        }

        public async Task<bool> CheckAvailabilityAsync(int menuItemId)
        {
            var menuItem = await _context.MenuItems
                .Where(m => m.Id == menuItemId)
                .Select(m => m.IsAvailable) 
                .FirstOrDefaultAsync();

            return menuItem; 
        }
    }
}