using AMB.Domain.Entities;

namespace AMB.Application.Interfaces.Repositories
{
    public interface IInventoryItemRepository
    {
        Task<InventoryItem> AddAsync(InventoryItem inventoryItem);
        Task<InventoryItem?> GetByIdAsync(int id);
        Task<InventoryItem?> UpdateAsync(InventoryItem inventoryItem);
        IQueryable<InventoryItem> GetSearchQuery();
        Task<List<UnitOfMeasure>> GetAllUoMsAsync();
        Task<List<Currency>> GetAllCurrenciesAsync();
    }
}
