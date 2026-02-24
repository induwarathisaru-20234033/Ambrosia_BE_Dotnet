using AMB.Application.Dtos;
using AMB.Application.Interfaces.Repositories;
using AMB.Application.Services;
using AMB.Application.Validators;
using AMB.Domain.Entities;
using AMB.Domain.Enums;
using AMB.Tests.Mocks;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AMB.Tests.RoleTests
{
    public class RoleServiceUpdateTests
    {
        private readonly TestRoleRepository _roleRepository;
        private readonly TestPermissionRepository _permissionRepository;
        private readonly RoleService _service;
        private readonly Role _existingRole;

        public RoleServiceUpdateTests()
        {
            _roleRepository = new TestRoleRepository();
            _permissionRepository = new TestPermissionRepository();

            var serviceProvider = new ServiceCollection()
                .AddScoped<IValidator<EditRoleRequestDto>>(sp =>
                    new EditRoleRequestValidator(sp))
                .AddScoped<IValidator<CreateRoleRequestDto>>(sp =>
                    new CreateRoleRequestValidator(sp))
                .AddScoped<IRoleRepository>(_ => _roleRepository)
                .BuildServiceProvider();

            _service = new RoleService(_roleRepository, _permissionRepository, serviceProvider);

            _existingRole = new Role
            {
                Id = 1,
                RoleCode = "FLOOR_MGR",
                RoleName = "Floor Manager",
                Description = "Original description",
                Status = 1, // Active
                RolePermissionMaps = new List<RolePermissionMap>
                {
                    new() { Id = 1, RoleId = 1, PermissionId = 1 },
                    new() { Id = 2, RoleId = 1, PermissionId = 2 }
                }
            };

            _roleRepository.Roles[1] = _existingRole;
        }

        [Fact]
        public async Task GetRoleByIdAsync_WithIncludePermissions_ReturnsRoleDetailDto()
        {
            // Act
            var result = await _service.GetRoleByIdAsync(1, true, true);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("FLOOR_MGR", result.RoleCode);
            Assert.Equal("Floor Manager", result.Name);
            Assert.Equal("Original description", result.Description);
            Assert.Equal(1, result.Status);

            Assert.Equal(2, result.SelectedPermissionIds.Count);
            Assert.Contains(1, result.SelectedPermissionIds);
            Assert.Contains(2, result.SelectedPermissionIds);

            Assert.NotNull(result.PermissionGroups);
            Assert.True(result.PermissionGroups.Count > 0);

            var empGroup = result.PermissionGroups.First(g => g.FeatureId == 1);
            Assert.Equal("Employee Management", empGroup.FeatureName);

            var viewEmpPerm = empGroup.Permissions.First(p => p.Id == 1);
            Assert.True(viewEmpPerm.IsSelected);

            var createEmpPerm = empGroup.Permissions.First(p => p.Id == 2);
            Assert.True(createEmpPerm.IsSelected);

            var deleteEmpPerm = empGroup.Permissions.First(p => p.Id == 4);
            Assert.False(deleteEmpPerm.IsSelected);
        }

        [Fact]
        public async Task GetRoleByIdAsync_WithInvalidId_ThrowsKeyNotFoundException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _service.GetRoleByIdAsync(999, true, true));
        }

        [Fact]
        public async Task UpdateRoleAsync_WithValidRequest_UpdatesRoleSuccessfully()
        {
            // Arrange
            var request = new EditRoleRequestDto
            {
                Id = 1,
                RoleCode = "FLOOR_MGR",
                Name = "Senior Floor Manager",
                Description = "Updated description",
                Status = 1,
                PermissionIds = new List<int> { 1, 2, 3, 5 }
            };

            // Act
            await _service.UpdateRoleAsync(request);

            // Assert
            Assert.NotNull(_roleRepository.LastUpdatedRole);
            Assert.NotNull(_roleRepository.LastUpdatedPermissionIds);

            Assert.Equal("Senior Floor Manager", _roleRepository.LastUpdatedRole!.RoleName);
            Assert.Equal("Updated description", _roleRepository.LastUpdatedRole.Description);
            Assert.Equal(1, _roleRepository.LastUpdatedRole.Status);

            Assert.Equal(4, _roleRepository.LastUpdatedPermissionIds!.Count);
            Assert.Contains(1, _roleRepository.LastUpdatedPermissionIds);
            Assert.Contains(2, _roleRepository.LastUpdatedPermissionIds);
            Assert.Contains(3, _roleRepository.LastUpdatedPermissionIds);
            Assert.Contains(5, _roleRepository.LastUpdatedPermissionIds);
        }

        [Fact]
        public async Task UpdateRoleAsync_WithInvalidId_ThrowsKeyNotFoundException()
        {
            // Arrange
            var request = new EditRoleRequestDto
            {
                Id = 999,
                Name = "Updated Name",
                Status = 1,
                PermissionIds = new List<int> { 1, 2 }
            };

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _service.UpdateRoleAsync(request));
        }

        [Fact]
        public async Task UpdateRoleAsync_WithInvalidRequest_ThrowsValidationException()
        {
            // Arrange
            var request = new EditRoleRequestDto
            {
                Id = 1,
                Name = "",
                Status = 1,
                PermissionIds = new List<int> { 1, 2 }
            };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(
                () => _service.UpdateRoleAsync(request));
        }

        [Fact]
        public async Task UpdateRoleAsync_WithNoPermissions_ThrowsValidationException()
        {
            // Arrange
            var request = new EditRoleRequestDto
            {
                Id = 1,
                Name = "Floor Manager",
                Status = 1,
                PermissionIds = new List<int>()
            };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(
                () => _service.UpdateRoleAsync(request));
        }

        [Fact]
        public async Task UpdateRoleAsync_WithInvalidPermissionIds_ThrowsInvalidOperationException()
        {
            // Arrange
            var request = new EditRoleRequestDto
            {
                Id = 1,
                Name = "Floor Manager",
                Status = 1,
                PermissionIds = new List<int> { 1, 2, 999 }
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.UpdateRoleAsync(request));
            Assert.Equal("One or more selected permissions are invalid.", exception.Message);
        }

        [Fact]
        public async Task UpdateRoleAsync_WithNewRoleCode_ThrowsValidationException()
        {
            // Arrange
            await _roleRepository.AddAsync(new Role
            {
                Id = 2,
                RoleCode = "BAR_MGR",
                RoleName = "Bar Manager",
                Status = 1,
                RolePermissionMaps = new List<RolePermissionMap>()
            });

            var request = new EditRoleRequestDto
            {
                Id = 1,
                RoleCode = "BAR_MGR",
                Name = "Floor Manager",
                Status = 1,
                PermissionIds = new List<int> { 1, 2 }
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(
                () => _service.UpdateRoleAsync(request));
            Assert.Contains("Role Code already exists", exception.Message);
        }

        [Fact]
        public async Task UpdateRoleAsync_WithStatusZero_UpdatesStatusToInactive()
        {
            // Arrange
            var request = new EditRoleRequestDto
            {
                Id = 1,
                Name = "Floor Manager",
                Status = 0,
                PermissionIds = new List<int> { 1, 2 }
            };

            // Act
            await _service.UpdateRoleAsync(request);

            // Assert
            Assert.NotNull(_roleRepository.LastUpdatedRole);
            Assert.Equal(0, _roleRepository.LastUpdatedRole!.Status);
        }

        [Fact]
        public async Task GetPermissionsGroupAsync_ReturnsPermissionsGroupedByFeature()
        {
            // Act
            var result = await _service.GetPermissionsGroupAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);

            var empGroup = result.First(g => g.FeatureId == 1);
            Assert.Equal("Employee Management", empGroup.FeatureName);
            Assert.Equal(4, empGroup.Permissions.Count);
            Assert.All(empGroup.Permissions, p => Assert.False(p.IsSelected));

            var roleGroup = result.First(g => g.FeatureId == 2);
            Assert.Equal("Role Management", roleGroup.FeatureName);
            Assert.Equal(4, roleGroup.Permissions.Count);
        }
    }
}