using AMB.Application.Dtos;
using AMB.Application.Interfaces.Repositories;
using AMB.Application.Interfaces.Services;
using AMB.Application.Mappers;
using AMB.Domain.Entities;
using AMB.Domain.Enums;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AMB.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RoleService> _logger;

        public RoleService(
            IRoleRepository roleRepository, 
            IPermissionRepository permissionRepository, 
            IServiceProvider serviceProvider, 
            ILogger<RoleService> logger)
        {
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task<RoleDto> CreateRoleAsync(CreateRoleRequestDto request)
        {
            try
            {
                var validator = _serviceProvider.GetRequiredService<IValidator<CreateRoleRequestDto>>();
                await validator.ValidateAndThrowAsync(request);

                var existingRole = await _roleRepository.GetByRoleCodeAsync(request.RoleCode);
                if (existingRole != null)
                {
                    throw new InvalidOperationException($"Role Code '{request.RoleCode}' already exists. Please choose a different code.");

                }

                var permissions = await _permissionRepository.GetByIdsAsync(request.PermissionIds);
                if (permissions.Count() != request.PermissionIds.Count)
                {
                    throw new InvalidOperationException("One or more selected permissions are invalid");
                }

                var role = request.ToRoleEntity();
                role.Status = (int)EntityStatus.Active;
                role.CreatedDate = DateTime.UtcNow;
                role.ModifiedDate = DateTime.UtcNow;

                role.RolePermissions = request.PermissionIds.Select(permissionId => new RolePermissionMap
                {
                    Role = role,
                    PermissionId = permissionId,
                    CreatedDate = DateTime.UtcNow
                }).ToList();

                var createdRole = await _roleRepository.AddAsync(role);

                return createdRole.ToRoleDto();
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error creating role with ID: {RoleCode}", request.RoleCode);
                throw;
            }
        }

        public async Task<bool> CheckRoleCodeExistsAsync(string RoleCode)
        {
            try
            {
                var role = await _roleRepository.GetByRoleCodeAsync(RoleCode);
                return role != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking role ID: {RoleCode}", RoleCode);
                throw;
            }
        }

        public async Task<IEnumerable<PermissionGroupDto>> GetPermissionsGroupAsync()
        {
            try
            {
                var permissions = await _permissionRepository.GetAllAsync();

                var grouped = permissions
                    .GroupBy(p => p.Module)
                    .Select(g => new PermissionGroupDto
                    {
                        Module = g.key,
                        FeatureName = g.key,
                        Permissions = g.Select(p => p.ToPermissionItemDto()).ToList()
                    }).ToList();
                return grouped;
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error retrieving permissions");
                throw;

            }
        }
    }
}
