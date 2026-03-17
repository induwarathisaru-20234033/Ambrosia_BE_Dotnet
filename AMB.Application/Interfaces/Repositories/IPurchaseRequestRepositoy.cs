using AMB.Domain.Entities;

namespace AMB.Application.Interfaces.Repositories
{
    public interface IPurchaseRequestRepositoy
    {
        Task<PurchaseRequest> AddAsync(PurchaseRequest purchaseRequest);
        Task<PurchaseRequest?> GetByIdAsync(int id);
        Task<PurchaseRequest?> UpdateAsync(PurchaseRequest purchaseRequest);
        Task<bool> ExistsByCodeAsync(string purchaseRequestCode);
        IQueryable<PurchaseRequest> GetSearchQuery();
        Task<Dictionary<string, string>> GetCreatorNamesByUsernamesAsync(List<string> creatorUsernames);
    }
}
