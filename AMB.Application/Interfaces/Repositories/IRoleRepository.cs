using AMB.Application.Dtos;
using AMB.Domain.Entities;

namespace AMB.Application.Interfaces.Repositories
{
    public interface IRoleRepository
    {
        Task<Role?> GetByIdAsync(int id, RoleQueryOptions? options = null);
        Task<Role> GetByRoleCodeAsync(string roleId);
        Task<List<Role>> GetAllAsync();
        Task<PaginatedResultDto<RoleDto>> GetAllRolesAsync(RoleFilterRequestDto filter);
        Task<Role> AddAsync(Role role);
        Task<CustomRole> AddCustomRoleAsync(CustomRole role);
        Task<CustomRole?> GetCustomRoleByIdAsync(int id, RoleQueryOptions? options = null);
        Task<Role> UpdateAsync(Role role);
        Task<Role> DeleteAsync(Role role);
        Task<bool> IsRoleCodeUniqueAsync(string roleCode, int? excludeId = null);
        Task<Role> UpdateWithPermissionsAsync(Role role, List<int> newPermissionIds);
        Task<RoleAssignedEmployeesDto?> GetAssignedEmployeesByRoleAsync(int roleId, bool isCustomRole = false);
        Task AssignRolesAsync(int roleId, List<int> employeeIds);
        Task UnassignRolesAsync(int roleId, List<int> employeeIds);
    }
    public class RoleQueryOptions
    {
        public bool IncludePermissions { get; set; } = false;
        public bool IncludePermissionFeatures { get; set; } = false;
    }
}
