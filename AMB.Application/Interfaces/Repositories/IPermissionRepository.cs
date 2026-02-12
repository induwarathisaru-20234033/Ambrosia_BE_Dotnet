using AMB.Application.Dtos;
using AMB.Domain.Entities;

namespace AMB.Application.Interfaces.Repositories
{
    public interface IPermissionRepository
    {
        Task<Permission> GetByIdAsync(int id);
        Task<List<Permission>> GetAllAsync();
        Task<List<Permission>> GetByIdsAsync(List<int> ids);
        Task<List<PermissionGroupDto>> GetPermissionsGroupedByFeatureAsync();
    }
}
