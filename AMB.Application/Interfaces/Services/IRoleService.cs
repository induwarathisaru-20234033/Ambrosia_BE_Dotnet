using AMB.Application.Dtos;

namespace AMB.Application.Interfaces.Services
{
    public interface IRoleService
    {
        Task<RoleDto> CreateRoleAsync(CreateRoleRequestDto request);
        Task<bool> CheckRoleCodeExistsAsync(string roleCode);
        Task<List<PermissionGroupDto>> GetPermissionsGroupAsync();
        Task UpdateRoleAsync(EditRoleRequestDto request);
        Task<RoleDetailDto> GetRoleByIdAsync(int id, bool includePermissions = false, bool includeFeatures = false); 
    }
}
