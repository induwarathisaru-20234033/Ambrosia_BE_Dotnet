using AMB.Application.Dtos;
using AMB.Application.Interfaces.Repositories;
using AMB.Application.Services;
using AMB.Application.Validators;
using AMB.Domain.Entities;
using AMB.Domain.Enums;
using AMB.Tests.Mocks;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AMB.Tests.EmployeeTests
{
    public class EmployeeServiceTests
    {
        [Fact]
        public async Task CreateEmployeeAsync_WithValidRequest_AddsEmployeeAndReturnsDto()
        {
            // Arrange a valid request and service dependencies.
            var repository = new TestEmployeeRepository();
            var authHelper = new TestAuthHelper("auth-123");
            var serviceProvider = new ServiceCollection()
                .AddScoped<IValidator<CreateEmployeeRequestDto>, CreateEmployeeValidator>()
                .BuildServiceProvider();

            var service = new EmployeeService(repository, serviceProvider, authHelper);
            var request = new CreateEmployeeRequestDto
            {
                EmployeeId = "EMP-001",
                FirstName = "Ana",
                LastName = "Silva",
                Email = "ana.silva@example.com",
                Username = "ana.silva@example.com",
                MobileNumber = "0712345678",
                Address = "123 Main Street",
                Password = "P@ssw0rd!"
            };

            // Act on the service call.
            var result = await service.CreateEmployeeAsync(request);

            // Assert repository writes and returned DTO fields.
            Assert.NotNull(repository.LastAddedEmployee);
            Assert.Equal((int)EntityStatus.Active, repository.LastAddedEmployee!.Status);
            Assert.Equal("auth-123", repository.LastAddedEmployee.UserId);
            Assert.Equal(request.EmployeeId, repository.LastAddedEmployee.EmployeeId);
            Assert.Equal(request.FirstName, repository.LastAddedEmployee.FirstName);
            Assert.Equal(request.LastName, repository.LastAddedEmployee.LastName);
            Assert.Equal(request.Email, repository.LastAddedEmployee.Email);
            Assert.Equal(request.Username, repository.LastAddedEmployee.Username);
            Assert.Equal(request.MobileNumber, repository.LastAddedEmployee.MobileNumber);
            Assert.Equal(request.Address, repository.LastAddedEmployee.Address);
            Assert.Equal("ana.silva@example.com", authHelper.LastUsername);
            Assert.Equal("P@ssw0rd!", authHelper.LastPassword);
            Assert.Equal("Ana Silva", authHelper.LastFullName);

            Assert.Equal(42, result.Id);
            Assert.Equal(request.EmployeeId, result.EmployeeId);
            Assert.Equal(request.FirstName, result.FirstName);
            Assert.Equal(request.LastName, result.LastName);
            Assert.Equal(request.Email, result.Email);
            Assert.Equal(request.Username, result.Username);
            Assert.Equal(request.MobileNumber, result.MobileNumber);
            Assert.Equal(request.Address, result.Address);
            Assert.False(result.IsOnline);
        }

        [Fact]
        public async Task CreateEmployeeAsync_WithInvalidRequest_ThrowsValidationException()
        {
            // Arrange an invalid request to trigger validation errors.
            var repository = new TestEmployeeRepository();
            var authHelper = new TestAuthHelper("auth-123");
            var serviceProvider = new ServiceCollection()
                .AddScoped<IValidator<CreateEmployeeRequestDto>, CreateEmployeeValidator>()
                .BuildServiceProvider();

            var service = new EmployeeService(repository, serviceProvider, authHelper);
            var request = new CreateEmployeeRequestDto
            {
                EmployeeId = "",
                FirstName = "",
                LastName = "",
                Email = "not-an-email",
                Username = "",
                MobileNumber = "123",
                Address = "",
                Password = "P@ssw0rd!"
            };

            // Act/Assert: validation should throw before any persistence.
            await Assert.ThrowsAsync<ValidationException>(() => service.CreateEmployeeAsync(request));
        }

        [Fact]
        public async Task GetEmployeeByIdAsync_WithExistingEmployee_ReturnsDtoWithStatus()
        {
            var repository = new TestEmployeeRepository();
            repository.Employees[5] = new AMB.Domain.Entities.Employee
            {
                Id = 5,
                EmployeeId = "EMP-005",
                FirstName = "Mia",
                LastName = "Perera",
                Email = "mia.perera@example.com",
                Username = "mia.perera@example.com",
                MobileNumber = "0711111111",
                Address = "45 Lake Road",
                Status = (int)EntityStatus.Inactive,
                CreatedDate = DateTimeOffset.UtcNow
            };

            var authHelper = new TestAuthHelper("auth-123");
            var serviceProvider = new ServiceCollection()
                .AddScoped<IValidator<CreateEmployeeRequestDto>, CreateEmployeeValidator>()
                .AddScoped<IValidator<UpdateEmployeeRequestDto>, UpdateEmployeeValidator>()
                .BuildServiceProvider();

            var service = new EmployeeService(repository, serviceProvider, authHelper);

            var result = await service.GetEmployeeByIdAsync(5);

            Assert.Equal(5, result.Id);
            Assert.Equal("EMP-005", result.EmployeeId);
            Assert.Equal("Mia", result.FirstName);
            Assert.Equal("Perera", result.LastName);
            Assert.Equal("mia.perera@example.com", result.Email);
            Assert.Equal("mia.perera@example.com", result.Username);
            Assert.Equal("0711111111", result.MobileNumber);
            Assert.Equal("45 Lake Road", result.Address);
            Assert.Equal((int)EntityStatus.Inactive, result.Status);
        }

        [Fact]
        public async Task GetEmployeeByIdAsync_WithMissingEmployee_ThrowsKeyNotFoundException()
        {
            var repository = new TestEmployeeRepository();
            var authHelper = new TestAuthHelper("auth-123");
            var serviceProvider = new ServiceCollection()
                .AddScoped<IValidator<CreateEmployeeRequestDto>, CreateEmployeeValidator>()
                .AddScoped<IValidator<UpdateEmployeeRequestDto>, UpdateEmployeeValidator>()
                .BuildServiceProvider();

            var service = new EmployeeService(repository, serviceProvider, authHelper);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetEmployeeByIdAsync(999));
        }

        [Fact]
        public async Task UpdateEmployeeAsync_WithValidRequest_UpdatesEmployeeStatusAndPassword()
        {
            var repository = new TestEmployeeRepository();
            repository.Employees[7] = new AMB.Domain.Entities.Employee
            {
                Id = 7,
                EmployeeId = "EMP-007",
                FirstName = "John",
                LastName = "Doe",
                MobileNumber = "0770000000",
                Username = "john.doe@example.com",
                Email = "john.doe@example.com",
                Address = "Old Address",
                Status = (int)EntityStatus.Active,
                UserId = "auth0|employee-7",
                CreatedDate = DateTimeOffset.UtcNow
            };

            var authHelper = new TestAuthHelper("auth-123");
            var serviceProvider = new ServiceCollection()
                .AddScoped<IValidator<CreateEmployeeRequestDto>, CreateEmployeeValidator>()
                .AddScoped<IValidator<UpdateEmployeeRequestDto>, UpdateEmployeeValidator>()
                .BuildServiceProvider();

            var service = new EmployeeService(repository, serviceProvider, authHelper);
            var request = new UpdateEmployeeRequestDto
            {
                Id = 7,
                EmployeeId = "EMP-007",
                FirstName = "Jane",
                LastName = "Doe",
                Email = "jane.doe@example.com",
                Username = "jane.doe@example.com",
                MobileNumber = "0771234567",
                Address = "New Address",
                Password = "N3wP@ssw0rd!",
                Status = EntityStatus.Inactive
            };

            var result = await service.UpdateEmployeeAsync(request);

            Assert.NotNull(result);
            Assert.Equal(7, result!.Id);
            Assert.Equal("Jane", result.FirstName);
            Assert.Equal("Doe", result.LastName);
            Assert.Equal("jane.doe@example.com", result.Email);
            Assert.Equal("jane.doe@example.com", result.Username);
            Assert.Equal("0771234567", result.MobileNumber);
            Assert.Equal("New Address", result.Address);
            Assert.Equal((int)EntityStatus.Inactive, result.Status);

            var updatedEmployee = repository.Employees[7];
            Assert.Equal("Jane", updatedEmployee.FirstName);
            Assert.Equal("Doe", updatedEmployee.LastName);
            Assert.Equal("jane.doe@example.com", updatedEmployee.Email);
            Assert.Equal("jane.doe@example.com", updatedEmployee.Username);
            Assert.Equal("0771234567", updatedEmployee.MobileNumber);
            Assert.Equal("New Address", updatedEmployee.Address);
            Assert.Equal((int)EntityStatus.Inactive, updatedEmployee.Status);
            Assert.Equal("auth0|employee-7", authHelper.LastUpdatedUserId);
            Assert.Equal("N3wP@ssw0rd!", authHelper.LastUpdatedPassword);
        }

        [Fact]
        public async Task UpdateEmployeeAsync_WithMissingEmployee_ThrowsKeyNotFoundException()
        {
            var repository = new TestEmployeeRepository();
            var authHelper = new TestAuthHelper("auth-123");
            var serviceProvider = new ServiceCollection()
                .AddScoped<IValidator<CreateEmployeeRequestDto>, CreateEmployeeValidator>()
                .AddScoped<IValidator<UpdateEmployeeRequestDto>, UpdateEmployeeValidator>()
                .BuildServiceProvider();

            var service = new EmployeeService(repository, serviceProvider, authHelper);
            var request = new UpdateEmployeeRequestDto
            {
                Id = 999,
                EmployeeId = "EMP-999",
                FirstName = "Ghost",
                LastName = "User",
                Email = "ghost.user@example.com",
                Username = "ghost.user@example.com",
                MobileNumber = "0712345678",
                Address = "Unknown",
                Password = "P@ssw0rd!",
                Status = EntityStatus.Active
            };

            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.UpdateEmployeeAsync(request));
        }

        [Fact]
        public async Task AssignRolesAsync_WithValidRequest_SavesMappings()
        {
            var repository = new TestEmployeeRepository();
            repository.Employees[7] = new AMB.Domain.Entities.Employee
            {
                Id = 7,
                EmployeeId = "EMP-007",
                FirstName = "John",
                LastName = "Doe",
                MobileNumber = "0770000000",
                Username = "john.doe@example.com",
                Email = "john.doe@example.com",
                Address = "Address",
                Status = 1
            };

            var authHelper = new TestAuthHelper("auth-123");
            var serviceProvider = new ServiceCollection()
                .AddScoped<IValidator<CreateEmployeeRequestDto>, CreateEmployeeValidator>()
                .AddScoped<IValidator<UpdateEmployeeRequestDto>, UpdateEmployeeValidator>()
                .BuildServiceProvider();

            var service = new EmployeeService(repository, serviceProvider, authHelper);

            var request = new AssignEmployeeRolesRequestDto
            {
                EmployeeId = 7,
                RoleIds = new List<int> { 1, 2 },
                CustomRoleIds = new List<int> { 10 }
            };

            await service.AssignRolesAsync(request);

            Assert.Equal(3, repository.EmployeeRoleMaps.Count);
            Assert.Equal(2, repository.EmployeeRoleMaps.Count(x => x.RoleId.HasValue));
            Assert.Equal(1, repository.EmployeeRoleMaps.Count(x => x.CustomRoleId.HasValue));
        }

        [Fact]
        public async Task UpdateEmployeeOnlineStatusAsync_WithExistingEmployee_UpdatesOnlineStatus()
        {
            var repository = new TestEmployeeRepository();
            repository.Employees[7] = new AMB.Domain.Entities.Employee
            {
                Id = 7,
                EmployeeId = "EMP-007",
                FirstName = "John",
                LastName = "Doe",
                MobileNumber = "0770000000",
                Username = "john.doe@example.com",
                Email = "john.doe@example.com",
                Address = "Address",
                Status = 1,
                IsOnline = false
            };

            var authHelper = new TestAuthHelper("auth-123");
            var serviceProvider = new ServiceCollection()
                .AddScoped<IValidator<CreateEmployeeRequestDto>, CreateEmployeeValidator>()
                .AddScoped<IValidator<UpdateEmployeeRequestDto>, UpdateEmployeeValidator>()
                .BuildServiceProvider();

            var service = new EmployeeService(repository, serviceProvider, authHelper);

            var result = await service.UpdateEmployeeOnlineStatusAsync(new UpdateEmployeeOnlineStatusRequestDto
            {
                Id = 7,
                IsOnline = true
            });

            Assert.True(result.IsOnline);
            Assert.True(repository.Employees[7].IsOnline);
        }

        [Fact]
        public async Task UpdateEmployeeOnlineStatusAsync_WithMissingEmployee_ThrowsKeyNotFoundException()
        {
            var repository = new TestEmployeeRepository();
            var authHelper = new TestAuthHelper("auth-123");
            var serviceProvider = new ServiceCollection()
                .AddScoped<IValidator<CreateEmployeeRequestDto>, CreateEmployeeValidator>()
                .AddScoped<IValidator<UpdateEmployeeRequestDto>, UpdateEmployeeValidator>()
                .BuildServiceProvider();

            var service = new EmployeeService(repository, serviceProvider, authHelper);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                service.UpdateEmployeeOnlineStatusAsync(new UpdateEmployeeOnlineStatusRequestDto
                {
                    Id = 999,
                    IsOnline = true
                }));
        }

        [Fact]
        public async Task GetWaitersWithCurrentAllocationsAsync_ReturnsWaitersWithReservationsAndTables()
        {
            var employeeRepository = new TestEmployeeRepository();
            var reservationRepository = new TestReservationRepository();

            employeeRepository.Employees[1] = new Employee
            {
                Id = 1,
                EmployeeId = "EMP-001",
                FirstName = "Ava",
                LastName = "Fernando",
                Username = "ava.fernando@example.com",
                MobileNumber = "0711111111",
                Address = "Address 1",
                Status = (int)EntityStatus.Active,
                IsOnline = true,
                EmployeeRoleMaps = new List<EmployeeRoleMap>
                {
                    new EmployeeRoleMap
                    {
                        Role = new Role { RoleName = AMB.Domain.Constants.Role.WaiterRole }
                    }
                }
            };

            employeeRepository.Employees[2] = new Employee
            {
                Id = 2,
                EmployeeId = "EMP-002",
                FirstName = "Ben",
                LastName = "Silva",
                Username = "ben.silva@example.com",
                MobileNumber = "0722222222",
                Address = "Address 2",
                Status = (int)EntityStatus.Active,
                IsOnline = false,
                EmployeeRoleMaps = new List<EmployeeRoleMap>
                {
                    new EmployeeRoleMap
                    {
                        Role = new Role { RoleName = AMB.Domain.Constants.Role.WaiterRole }
                    }
                }
            };

            reservationRepository.Reservations[101] = new Reservation
            {
                Id = 101,
                ReservationCode = "RES-101",
                PartySize = 4,
                ReservationStatus = (int)ReservationStatus.Arrived,
                ReservationDate = DateTimeOffset.UtcNow.AddHours(1),
                AssignedWaiterId = 1,
                TableId = 11,
                Table = new Table { Id = 11, TableName = "T1", Capacity = 4 },
                Status = (int)EntityStatus.Active
            };

            reservationRepository.Reservations[102] = new Reservation
            {
                Id = 102,
                ReservationCode = "RES-102",
                PartySize = 2,
                ReservationStatus = (int)ReservationStatus.Booked,
                ReservationDate = DateTimeOffset.UtcNow.AddHours(2),
                AssignedWaiterId = 1,
                TableId = 12,
                Table = new Table { Id = 12, TableName = "T2", Capacity = 2 },
                Status = (int)EntityStatus.Active
            };

            reservationRepository.Reservations[103] = new Reservation
            {
                Id = 103,
                ReservationCode = "RES-103",
                PartySize = 6,
                ReservationStatus = (int)ReservationStatus.Cancelled,
                ReservationDate = DateTimeOffset.UtcNow.AddHours(3),
                AssignedWaiterId = 2,
                TableId = 13,
                Table = new Table { Id = 13, TableName = "T3", Capacity = 6 },
                Status = (int)EntityStatus.Active
            };

            var authHelper = new TestAuthHelper("auth-123");
            var serviceProvider = new ServiceCollection()
                .AddScoped<IValidator<CreateEmployeeRequestDto>, CreateEmployeeValidator>()
                .AddScoped<IValidator<UpdateEmployeeRequestDto>, UpdateEmployeeValidator>()
                .AddScoped<IReservationRepository>(_ => reservationRepository)
                .BuildServiceProvider();

            var service = new EmployeeService(employeeRepository, serviceProvider, authHelper);

            var result = await service.GetWaitersWithCurrentAllocationsAsync();

            Assert.Equal(2, result.Count);

            var ava = result.Single(r => r.WaiterId == 1);
            Assert.Equal(2, ava.AllocatedReservationCount);
            Assert.Equal(2, ava.AllocatedTableCount);
            Assert.Equal(new[] { "T1", "T2" }, ava.Tables.Select(t => t.TableName).ToArray());

            var ben = result.Single(r => r.WaiterId == 2);
            Assert.Equal(0, ben.AllocatedReservationCount);
            Assert.Equal(0, ben.AllocatedTableCount);
        }
    }
}
