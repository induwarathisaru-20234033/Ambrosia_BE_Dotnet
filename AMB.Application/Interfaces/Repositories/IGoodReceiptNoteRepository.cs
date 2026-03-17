using AMB.Domain.Entities;

namespace AMB.Application.Interfaces.Repositories
{
    public interface IGoodReceiptNoteRepository
    {
        Task<GoodReceiptNote> AddAsync(GoodReceiptNote goodReceiptNote);
        Task<List<PurchaseRequestItem>> GetPurchaseRequestItemsByIdsAsync(List<int> prItemIds);
        Task<bool> ExistsByNumberAsync(string grnNumber);
    }
}
