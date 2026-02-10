using AMB.Domain.Entities;

namespace AMB.Application.Interfaces.Repositories
{
    public interface IPermissionRepository
    {
        Task<Permission> GetByIdAsync(int id);
        Task<IEnumerable<Permission>> GetAllAsync();
        Task<IEnumerable<Permission>> GetByIdsAsync(List<int> ids);
    }
}
