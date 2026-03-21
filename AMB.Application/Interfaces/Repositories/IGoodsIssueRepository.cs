using AMB.Domain.Entities;

namespace AMB.Application.Interfaces.Repositories
{
    public interface IGoodsIssueRepository
    {
        Task<GoodIssueNote> AddAsync(GoodIssueNote goodIssueNote);
        Task<GoodIssueNote?> GetByIdAsync(int id);
        Task<GoodIssueNote?> UpdateAsync(GoodIssueNote goodIssueNote);
        Task<List<InventoryItem>> GetInventoryItemsByIdsAsync(List<int> inventoryItemIds);
        Task<bool> ExistsByNumberAsync(string giNumber);
        IQueryable<GoodIssueNote> GetSearchQuery();
    }
}
