using AMB.Domain.Entities;

namespace AMB.Application.Interfaces.Repositories
{
    public interface IRoleRepository
    {
        Task<Role?> GetByIdAsync(int id, RoleQueryOptions options = null);
        Task<Role> GetByRoleCodeAsync(string roleId);
        Task<List<Role>> GetAllAsync();
        Task<Role> AddAsync(Role role);
        Task<Role> UpdateAsync(Role role);
        Task<Role> DeleteAsync(Role role);
        Task<bool> IsRoleCodeUniqueAsync(string roleCode, int? excludeId = null);
        Task<Role> UpdateWithPermissionsAsync(Role role, List<int> newPermissionIds);
    }
    public class RoleQueryOptions
    {
        public bool IncludePermissions { get; set; } = false;
        public bool IncludePermissionFeatures { get; set; } = false;
    }
}
