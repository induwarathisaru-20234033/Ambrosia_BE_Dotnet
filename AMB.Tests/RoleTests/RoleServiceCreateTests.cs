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
    public class RoleServiceCreateTests
    {
        [Fact]
        public async Task CreateRoleAsync_WithValidRequest_AddsRoleAndReturnsDto()
        {
            // Arrange
            var roleRepository = new TestRoleRepository();
            var permissionRepository = new TestPermissionRepository();
            var serviceProvider = new ServiceCollection()
                .AddScoped<IValidator<CreateRoleRequestDto>>(sp =>
                    new CreateRoleRequestValidator(sp))
                .AddScoped<IRoleRepository>(_ => roleRepository)
                .BuildServiceProvider();

            var service = new RoleService(roleRepository, permissionRepository, serviceProvider);

            var request = new CreateRoleRequestDto
            {
                RoleCode = "FLOOR_MGR",
                Name = "Floor Manager",
                Description = "Manages floor operations",
                Status = 1,  // int, not string
                PermissionIds = new List<int> { 1, 2, 5, 6 }
            };

            // Act
            var result = await service.CreateRoleAsync(request);

            // Assert
            Assert.NotNull(roleRepository.LastAddedRole);

            Assert.Equal("FLOOR_MGR", roleRepository.LastAddedRole!.RoleCode);
            Assert.Equal("Floor Manager", roleRepository.LastAddedRole.RoleName);
            Assert.Equal("Manages floor operations", roleRepository.LastAddedRole.Description);
            Assert.Equal(1, roleRepository.LastAddedRole.Status); // Status is int

            // Check permission mappings were created
            Assert.NotNull(roleRepository.LastAddedRole.RolePermissionMaps);
            Assert.Equal(4, roleRepository.LastAddedRole.RolePermissionMaps!.Count);

            // Get the actual permission IDs from the maps
            var actualPermissionIds = roleRepository.LastAddedRole.RolePermissionMaps
                .Select(rpm => rpm.PermissionId).ToList();

            Assert.Contains(1, actualPermissionIds);
            Assert.Contains(2, actualPermissionIds);
            Assert.Contains(5, actualPermissionIds);
            Assert.Contains(6, actualPermissionIds);

            Assert.NotNull(result);
            Assert.Equal(roleRepository.LastAddedRole.Id, result.Id);
            Assert.Equal("FLOOR_MGR", result.RoleCode);
            Assert.Equal("Floor Manager", result.Name);
            Assert.Equal("Manages floor operations", result.Description);
            Assert.Equal(1, result.Status); // Status is int
            Assert.Equal(4, result.Permissions.Count);
        }

        [Fact]
        public async Task CreateRoleAsync_WithInvalidRequest_ThrowsValidationException()
        {
            // Arrange
            var roleRepository = new TestRoleRepository();
            var permissionRepository = new TestPermissionRepository();
            var serviceProvider = new ServiceCollection()
                .AddScoped<IValidator<CreateRoleRequestDto>>(sp =>
                    new CreateRoleRequestValidator(sp))
                .AddScoped<IRoleRepository>(_ => roleRepository)
                .BuildServiceProvider();

            var service = new RoleService(roleRepository, permissionRepository, serviceProvider);

            var request = new CreateRoleRequestDto
            {
                RoleCode = "",
                Name = "",
                Description = "This is a test description",
                Status = 1,
                PermissionIds = new List<int>()
            };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => service.CreateRoleAsync(request));
        }

        [Fact]
        public async Task CreateRoleAsync_WithDuplicateRoleCode_ThrowsValidationException()
        {
            // Arrange
            var roleRepository = new TestRoleRepository();
            var permissionRepository = new TestPermissionRepository();

            await roleRepository.AddAsync(new Role
            {
                Id = 1,
                RoleCode = "FLOOR_MGR",
                RoleName = "Existing Floor Manager",
                Status = 1,
                RolePermissionMaps = new List<RolePermissionMap>()
            });

            var serviceProvider = new ServiceCollection()
                .AddScoped<IValidator<CreateRoleRequestDto>>(sp =>
                    new CreateRoleRequestValidator(sp))
                .AddScoped<IRoleRepository>(_ => roleRepository)
                .BuildServiceProvider();

            var service = new RoleService(roleRepository, permissionRepository, serviceProvider);

            var request = new CreateRoleRequestDto
            {
                RoleCode = "FLOOR_MGR",
                Name = "New Floor Manager",
                Status = 1,
                PermissionIds = new List<int> { 1, 2 }
            };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => service.CreateRoleAsync(request));
        }

        [Fact]
        public async Task CreateRoleAsync_WithInvalidPermissionIds_ThrowsInvalidOperationException()
        {
            // Arrange
            var roleRepository = new TestRoleRepository();
            var permissionRepository = new TestPermissionRepository();
            var serviceProvider = new ServiceCollection()
                .AddScoped<IValidator<CreateRoleRequestDto>>(sp =>
                    new CreateRoleRequestValidator(sp))
                .AddScoped<IRoleRepository>(_ => roleRepository)
                .BuildServiceProvider();

            var service = new RoleService(roleRepository, permissionRepository, serviceProvider);

            var request = new CreateRoleRequestDto
            {
                RoleCode = "FLOOR_MGR",
                Name = "Floor Manager",
                Status = 1,
                PermissionIds = new List<int> { 1, 2, 999 }
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.CreateRoleAsync(request));
            Assert.Equal("One or more selected permissions are invalid.", exception.Message);
        }

        [Fact]
        public async Task CreateRoleAsync_WithStatusZero_CreatesRoleWithInactiveStatus()
        {
            // Arrange
            var roleRepository = new TestRoleRepository();
            var permissionRepository = new TestPermissionRepository();
            var serviceProvider = new ServiceCollection()
                .AddScoped<IValidator<CreateRoleRequestDto>>(sp =>
                    new CreateRoleRequestValidator(sp))
                .AddScoped<IRoleRepository>(_ => roleRepository)
                .BuildServiceProvider();

            var service = new RoleService(roleRepository, permissionRepository, serviceProvider);

            var request = new CreateRoleRequestDto
            {
                RoleCode = "FLOOR_MGR",
                Name = "Floor Manager",
                Status = 0,  // Inactive
                PermissionIds = new List<int> { 1, 2 }
            };

            // Act
            var result = await service.CreateRoleAsync(request);

            // Assert
            Assert.NotNull(roleRepository.LastAddedRole);
            Assert.Equal(0, roleRepository.LastAddedRole!.Status);
            Assert.Equal(0, result.Status);
        }
    }
}