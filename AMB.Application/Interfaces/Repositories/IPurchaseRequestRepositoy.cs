using AMB.Domain.Entities;

namespace AMB.Application.Interfaces.Repositories
{
    public interface IPurchaseRequestRepositoy
    {
        Task<PurchaseRequest> AddAsync(PurchaseRequest purchaseRequest);
        Task<PurchaseRequest?> GetByIdAsync(int id);
        Task<PurchaseRequest?> UpdateAsync(PurchaseRequest purchaseRequest);
        Task<bool> ExistsByCodeAsync(string purchaseRequestCode);
    }
}
