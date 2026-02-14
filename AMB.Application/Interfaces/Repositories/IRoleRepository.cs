using AMB.Domain.Entities;

namespace AMB.Application.Interfaces.Repositories
{
    public interface IRoleRepository
    {
        Task<Role> GetByIdAsync(int  id);
        Task<Role> GetByRoleCodeAsync(string roleId);
        Task<List<Role>> GetAllAsync();
        Task<Role> AddAsync(Role role);
        Task<Role> UpdateAsync(Role role);
        Task<Role> DeleteAsync(Role role);
        Task<Role?> GetByIdWithPermissionsAsync(int id);
        Task<bool> IsRoleCodeUniqueAsync(string roleCode, int? excludeId = null);
    }
}
