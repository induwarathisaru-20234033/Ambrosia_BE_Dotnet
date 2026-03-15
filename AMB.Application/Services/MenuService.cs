using AMB.Application.Dtos;
using AMB.Application.Interfaces.Repositories;
using AMB.Application.Interfaces.Services;
using AMB.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AMB.Application.Services
{
    public class MenuService : IMenuService
    {
        private readonly IMenuRepository _menuRepository;

        public MenuService(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public async Task<MenuItemDto> AddMenuItem(CreateMenuItemDto dto)
        {
            var entity = new MenuItem
            {
                Name = dto.Name,
                Price = dto.Price,
                Category = dto.Category,
                IsAvailable = dto.IsAvailable
            };

            await _menuRepository.AddAsync(entity);

            return new MenuItemDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Price = entity.Price,
                Category = entity.Category,
                IsAvailable = entity.IsAvailable
            };
        }

        public async Task<List<MenuItemDto>> GetMenuItems(
            string? category,
            string? name,
            bool? isAvailable)
        {
            var query = _menuRepository.GetQuery();

            if (!string.IsNullOrEmpty(category))
                query = query.Where(x => x.Category == category);

            if (!string.IsNullOrEmpty(name))
                query = query.Where(x => x.Name.Contains(name));

            if (isAvailable.HasValue)
                query = query.Where(x => x.IsAvailable == isAvailable.Value);

            var items = await query.ToListAsync();

            return items.Select(x => new MenuItemDto
            {
                Id = x.Id,
                Name = x.Name,
                Price = x.Price,
                Category = x.Category,
                IsAvailable = x.IsAvailable
            }).ToList();
        }
    }
}