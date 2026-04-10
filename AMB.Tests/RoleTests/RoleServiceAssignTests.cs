using AMB.Application.Dtos;
using AMB.Application.Interfaces.Repositories;
using AMB.Application.Services;
using AMB.Application.Validators;
using AMB.Domain.Entities;
using AMB.Tests.Mocks;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AMB.Tests.RoleTests
{
    public class RoleServiceAssignTests
    {
        private static (RoleService service, TestRoleRepository roleRepository) BuildService()
        {
            var roleRepository = new TestRoleRepository();
            var permissionRepository = new TestPermissionRepository();
            var serviceProvider = new ServiceCollection()
                .AddScoped<IValidator<CreateRoleRequestDto>>(sp =>
                    new CreateRoleRequestValidator(sp))
                .AddScoped<IRoleRepository>(_ => roleRepository)
                .BuildServiceProvider();

            var service = new RoleService(roleRepository, permissionRepository, serviceProvider);
            return (service, roleRepository);
        }

        // ─── AssignRolesAsync ───────────────────────────────────────────────────

        [Fact]
        public async Task AssignRolesAsync_WithValidRequest_CallsRepository()
        {
            // Arrange
            var (service, roleRepository) = BuildService();
            roleRepository.Roles[1] = new Role { Id = 1, RoleCode = "MGR", RoleName = "Manager" };

            var request = new AssignRoleRequestDto
            {
                RoleId = 1,
                EmployeeIds = new List<int> { 10, 20, 30 }
            };

            // Act
            await service.AssignRolesAsync(request);

            // Assert
            Assert.Equal(1, roleRepository.LastAssignedRoleId);
            Assert.NotNull(roleRepository.LastAssignedEmployeeIds);
            Assert.Equal(3, roleRepository.LastAssignedEmployeeIds!.Count);
            Assert.Contains(10, roleRepository.LastAssignedEmployeeIds);
            Assert.Contains(20, roleRepository.LastAssignedEmployeeIds);
            Assert.Contains(30, roleRepository.LastAssignedEmployeeIds);
        }

        [Fact]
        public async Task AssignRolesAsync_WithDuplicateEmployeeIds_DeduplicatesBeforeCalling()
        {
            // Arrange
            var (service, roleRepository) = BuildService();
            roleRepository.Roles[1] = new Role { Id = 1, RoleCode = "MGR", RoleName = "Manager" };

            var request = new AssignRoleRequestDto
            {
                RoleId = 1,
                EmployeeIds = new List<int> { 10, 10, 20 }
            };

            // Act
            await service.AssignRolesAsync(request);

            // Assert
            Assert.NotNull(roleRepository.LastAssignedEmployeeIds);
            Assert.Equal(2, roleRepository.LastAssignedEmployeeIds!.Count);
            Assert.Contains(10, roleRepository.LastAssignedEmployeeIds);
            Assert.Contains(20, roleRepository.LastAssignedEmployeeIds);
        }

        [Fact]
        public async Task AssignRolesAsync_WithInvalidRoleId_ThrowsArgumentException()
        {
            // Arrange
            var (service, _) = BuildService();

            var request = new AssignRoleRequestDto
            {
                RoleId = 0,
                EmployeeIds = new List<int> { 10 }
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.AssignRolesAsync(request));
        }

        [Fact]
        public async Task AssignRolesAsync_WithEmptyEmployeeIds_ThrowsArgumentException()
        {
            // Arrange
            var (service, _) = BuildService();

            var request = new AssignRoleRequestDto
            {
                RoleId = 1,
                EmployeeIds = new List<int>()
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.AssignRolesAsync(request));
        }

        [Fact]
        public async Task AssignRolesAsync_WithNonExistentRole_ThrowsKeyNotFoundException()
        {
            // Arrange
            var (service, _) = BuildService();

            var request = new AssignRoleRequestDto
            {
                RoleId = 999,
                EmployeeIds = new List<int> { 10 }
            };

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.AssignRolesAsync(request));
        }

        // ─── UnassignRolesAsync ─────────────────────────────────────────────────

        [Fact]
        public async Task UnassignRolesAsync_WithValidRequest_CallsRepository()
        {
            // Arrange
            var (service, roleRepository) = BuildService();
            roleRepository.Roles[2] = new Role { Id = 2, RoleCode = "CHEF", RoleName = "Chef" };

            var request = new AssignRoleRequestDto
            {
                RoleId = 2,
                EmployeeIds = new List<int> { 5, 6 }
            };

            // Act
            await service.UnassignRolesAsync(request);

            // Assert
            Assert.Equal(2, roleRepository.LastUnassignedRoleId);
            Assert.NotNull(roleRepository.LastUnassignedEmployeeIds);
            Assert.Equal(2, roleRepository.LastUnassignedEmployeeIds!.Count);
            Assert.Contains(5, roleRepository.LastUnassignedEmployeeIds);
            Assert.Contains(6, roleRepository.LastUnassignedEmployeeIds);
        }

        [Fact]
        public async Task UnassignRolesAsync_WithDuplicateEmployeeIds_DeduplicatesBeforeCalling()
        {
            // Arrange
            var (service, roleRepository) = BuildService();
            roleRepository.Roles[2] = new Role { Id = 2, RoleCode = "CHEF", RoleName = "Chef" };

            var request = new AssignRoleRequestDto
            {
                RoleId = 2,
                EmployeeIds = new List<int> { 5, 5, 6 }
            };

            // Act
            await service.UnassignRolesAsync(request);

            // Assert
            Assert.NotNull(roleRepository.LastUnassignedEmployeeIds);
            Assert.Equal(2, roleRepository.LastUnassignedEmployeeIds!.Count);
        }

        [Fact]
        public async Task UnassignRolesAsync_WithInvalidRoleId_ThrowsArgumentException()
        {
            // Arrange
            var (service, _) = BuildService();

            var request = new AssignRoleRequestDto
            {
                RoleId = -1,
                EmployeeIds = new List<int> { 5 }
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.UnassignRolesAsync(request));
        }

        [Fact]
        public async Task UnassignRolesAsync_WithEmptyEmployeeIds_ThrowsArgumentException()
        {
            // Arrange
            var (service, _) = BuildService();

            var request = new AssignRoleRequestDto
            {
                RoleId = 2,
                EmployeeIds = new List<int>()
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.UnassignRolesAsync(request));
        }

        [Fact]
        public async Task UnassignRolesAsync_WithNonExistentRole_ThrowsKeyNotFoundException()
        {
            // Arrange
            var (service, _) = BuildService();

            var request = new AssignRoleRequestDto
            {
                RoleId = 888,
                EmployeeIds = new List<int> { 5 }
            };

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.UnassignRolesAsync(request));
        }
    }
}
