using AMB.Application.Dtos;
using AMB.Application.Interfaces.Repositories;
using AMB.Application.Interfaces.Services;
using AMB.Application.Mappers;
using AMB.Domain.Entities;
using AMB.Domain.Enums;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AMB.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IServiceProvider _serviceProvider;

        public RoleService(
            IRoleRepository roleRepository,
            IPermissionRepository permissionRepository,
            IServiceProvider serviceProvider)
        {
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
            _serviceProvider = serviceProvider;
        }

        public async Task<RoleDto> CreateRoleAsync(CreateRoleRequestDto request)
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<CreateRoleRequestDto>>();
            await validator.ValidateAndThrowAsync(request);


            var permissions = await _permissionRepository.GetByIdsAsync(request.PermissionIds);
            if (permissions.Count != request.PermissionIds.Count)
            {
                throw new InvalidOperationException("One or more selected permissions are invalid.");
            }

            // Create role entity from DTO
            var role = request.ToRoleEntity();

            role.RoleCode = request.RoleCode.ToUpper();
            role.Status = request.Status == "ENABLED" ? (int)EntityStatus.Active : (int)EntityStatus.Inactive;

            // Add role permission mappings
            role.RolePermissionMaps = request.PermissionIds.Select(permissionId => new RolePermissionMap
            {
                PermissionId = permissionId,
                Role = role
            }).ToList();

            // Save to db
            var createdRole = await _roleRepository.AddAsync(role);

            var roleWithPermissions = await _roleRepository.GetByIdWithPermissionsAsync(createdRole.Id);

            // Map to DTO
            var roleDto = roleWithPermissions.ToRoleDto();

            if (roleWithPermissions?.RolePermissionMaps != null)
            {
                roleDto.Permissions = roleWithPermissions.RolePermissionMaps
                    .Where(rpm => rpm.Permission != null)
                    .Select(rpm => new PermissionDto
                    {
                        Id = rpm.Permission!.Id,
                        PermissionCode = rpm.Permission.PermissionCode,
                        Name = rpm.Permission.PermissionName,
                        FeatureId = rpm.Permission.FeatureId,
                        FeatureName = rpm.Permission.Feature?.FeatureName ?? string.Empty,
                        FeatureCode = rpm.Permission.Feature?.FeatureCode ?? string.Empty
                    }).ToList();
            }

            return roleDto;
        }

        public async Task<bool> CheckRoleCodeExistsAsync(string roleCode)
        {
            try
            {
                return !await _roleRepository.IsRoleCodeUniqueAsync(roleCode.ToUpper());
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<PermissionGroupDto>> GetPermissionsGroupAsync()
        {
            return await _permissionRepository.GetPermissionsGroupedByFeatureAsync();
        }
    }
}