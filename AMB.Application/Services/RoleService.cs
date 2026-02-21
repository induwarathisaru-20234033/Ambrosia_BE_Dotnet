using AMB.Application.Dtos;
using AMB.Application.Interfaces.Repositories;
using AMB.Application.Interfaces.Services;
using AMB.Application.Mappers;
using AMB.Domain.Entities;
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

            var role = request.ToRoleEntity();

            role.RoleCode = request.RoleCode.ToUpper();
            role.Status = request.Status;

            role.RolePermissionMaps = request.PermissionIds.Select(permissionId => new RolePermissionMap
            {
                PermissionId = permissionId,
                Role = role
            }).ToList();

            var createdRole = await _roleRepository.AddAsync(role);

            var options = new RoleQueryOptions
            {
                IncludePermissions = true,
                IncludePermissionFeatures = true
            };
            var roleWithPermissions = await _roleRepository.GetByIdAsync(createdRole.Id, options);

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
            var permissions = await _permissionRepository.GetPermissionsWithFeaturesAsync();

            return permissions
                .GroupBy(p => new {
                    FeatureId = p.FeatureId,
                    FeatureName = p.Feature.FeatureName,
                    FeatureCode = p.Feature.FeatureCode
                })
                .Select(g => new PermissionGroupDto
                {
                    FeatureId = g.Key.FeatureId,
                    FeatureName = g.Key.FeatureName, 
                    FeatureCode = g.Key.FeatureCode,  
                    Permissions = g.Select(p => new PermissionItemDto
                    {
                        Id = p.Id,
                        PermissionCode = p.PermissionCode,
                        Name = p.PermissionName
                    }).ToList()
                })
                .ToList();
        }

        public async Task<RoleDetailDto> GetRoleByIdAsync(int id, bool includePermissions = false, bool includeFeatures = false)
        {
            var options = new RoleQueryOptions
            {
                IncludePermissions = includePermissions,
                IncludePermissionFeatures = includeFeatures
            };

            var role = await _roleRepository.GetByIdAsync(id, options);
            if (role == null)
            {
                throw new KeyNotFoundException($"Role with ID {id} not found");
            }

            // Permissions for grouping
            var allPermissions = await _permissionRepository.GetPermissionsWithFeaturesAsync();

            var selectedPermissionIds = role.RolePermissionMaps?
                .Select(rpm => rpm.PermissionId)
                .ToList() ?? new List<int>();

            var permissionGroups = allPermissions
                .GroupBy(p => new {
                    FeatureId = p.FeatureId,
                    FeatureName = p.Feature?.FeatureName ?? "Unknown",
                    FeatureCode = p.Feature?.FeatureCode ?? "UNKNOWN"
                })
                .Select(g => new PermissionGroupDto
                {
                    FeatureId = g.Key.FeatureId,
                    FeatureName = g.Key.FeatureName,
                    FeatureCode = g.Key.FeatureCode,
                    Permissions = g.Select(p => new PermissionItemDto
                    {
                        Id = p.Id,
                        PermissionCode = p.PermissionCode,
                        Name = p.PermissionName,
                        IsSelected = selectedPermissionIds.Contains(p.Id)
                    }).ToList()
                })
                .ToList();

            return new RoleDetailDto
            {
                Id = role.Id,
                RoleCode = role.RoleCode,
                Name = role.RoleName,
                Description = role.Description,
                Status = role.Status,
                SelectedPermissionIds = selectedPermissionIds,
                PermissionGroups = permissionGroups,
                CreatedDate = role.CreatedDate,
                UpdatedDate = role.UpdatedDate
            };
        }


        // Edit Role
        public async Task UpdateRoleAsync(EditRoleRequestDto request)
        {
            // Validate
            var validator = _serviceProvider.GetRequiredService<IValidator<EditRoleRequestDto>>();
            await validator.ValidateAndThrowAsync(request);

            var existingRole = await _roleRepository.GetByIdAsync(request.Id);
            if (existingRole == null)
            {
                throw new KeyNotFoundException($"Role with ID {request.Id} not found");
            }

            // Validate permissions exist
            var permissions = await _permissionRepository.GetByIdsAsync(request.PermissionIds);
            if (permissions.Count != request.PermissionIds.Count)
            {
                throw new InvalidOperationException("One or more selected permissions are invalid.");
            }

            // Update role properties
            existingRole.RoleName = request.Name;
            existingRole.Description = request.Description;
            existingRole.Status = request.Status;

            await _roleRepository.UpdateWithPermissionsAsync(existingRole, request.PermissionIds);

        }

    }
}