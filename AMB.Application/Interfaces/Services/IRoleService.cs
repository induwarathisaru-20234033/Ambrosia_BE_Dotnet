using AMB.Application.Dtos;

namespace AMB.Application.Interfaces.Services
{
    public interface IRoleService
    {
        Task<RoleDto> CreateRoleAsync(CreateRoleRequestDto request);
        Task<bool> CheckRoleCodeExistsAsync(string roleCode);
        Task<List<PermissionGroupDto>> GetPermissionsGroupAsync();
        Task<PaginatedResultDto<RoleDto>> GetAllRolesAsync(RoleFilterRequestDto filter);
        Task UpdateRoleAsync(EditRoleRequestDto request);
        Task<RoleDetailDto> GetRoleByIdAsync(int id, bool includePermissions = false, bool includeFeatures = false);
        Task<RoleAssignedEmployeesDto> GetAssignedEmployeesByRoleAsync(int roleId, bool isCustomRole = false);
        Task AssignRolesAsync(AssignRoleRequestDto request);
        Task UnassignRolesAsync(AssignRoleRequestDto request);
    }
}
