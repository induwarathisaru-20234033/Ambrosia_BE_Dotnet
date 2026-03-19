using AMB.Domain.Entities;

namespace AMB.Application.Interfaces.Repositories
{
    public interface IGoodReceiptNoteRepository
    {
        Task<GoodReceiptNote> AddAsync(GoodReceiptNote goodReceiptNote);
        Task<GoodReceiptNote?> GetByIdAsync(int id);
        Task<GoodReceiptNote?> UpdateAsync(GoodReceiptNote goodReceiptNote);
        Task<List<PurchaseRequestItem>> GetPurchaseRequestItemsByIdsAsync(List<int> prItemIds);
        Task<bool> ExistsByNumberAsync(string grnNumber);
        Task ProcessPostedGrnAsync(int grnId);
        IQueryable<GoodReceiptNote> GetSearchQuery();
    }
}
