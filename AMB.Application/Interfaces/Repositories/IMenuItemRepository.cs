using AMB.Domain.Entities;

namespace AMB.Application.Interfaces.Repositories
{
    public interface IMenuItemRepository
    {
        Task<MenuItem?> GetByIdAsync(int id);
        Task<List<MenuItem>> GetByIdsAsync(List<int> ids);
        Task<List<MenuItem>> GetByCategoryAsync(string category);
        IQueryable<MenuItem> GetQuery();
        Task<bool> CheckAvailabilityAsync(int menuItemId);
    }
}