using AMB.Application.Interfaces.Repositories;
using AMB.Domain.Entities;
using AMB.Infra.DBContexts;
using Microsoft.EntityFrameworkCore;

public class MenuRepository : IMenuRepository
{
    private readonly AMBContext _context;

    public MenuRepository(AMBContext context)
    {
        _context = context;
    }

    public async Task<MenuItem> AddAsync(MenuItem item)
    {
        _context.MenuItems.Add(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public IQueryable<MenuItem> GetQuery()
    {
        return _context.MenuItems.AsQueryable();
    }
}