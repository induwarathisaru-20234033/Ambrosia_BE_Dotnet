using AMB.Application.Dtos;
using AMB.Application.Services;
using AMB.Application.Validators;
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
    }
}
