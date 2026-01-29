using AMB.Application.Dtos;
using AMB.Application.Interfaces.Repositories;
using AMB.Application.Interfaces.Services;
using AMB.Domain.Entities;

namespace AMB.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public AuthService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository; 
        }

        public async Task<LoginResponseDto> LoginAsync(string authUserId) 
        {
            var employee = await _employeeRepository.GetByUserIDAsync(authUserId);

            if (employee == null)
            {
                throw new UnauthorizedAccessException("Employee not found.");
            }

            var roles = employee.EmployeeRoleMaps?
                .Select(map => map.Role?.RoleCode)
                .Where(r => !string.IsNullOrWhiteSpace(r))
                .ToList() ?? new List<string>();

            var permissions = employee.EmployeeRoleMaps?
                .SelectMany(erm => erm.Role?.RolePermissionMaps ?? Enumerable.Empty<RolePermissionMap>())
                .Select(rpm => rpm.Permission?.PermissionCode)
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Distinct()
                .ToList() ?? new List<string>();

            return new LoginResponseDto
            {
                Username = employee.Username,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                Roles = roles,
                Permissions = permissions
            };
        }
    }
}
