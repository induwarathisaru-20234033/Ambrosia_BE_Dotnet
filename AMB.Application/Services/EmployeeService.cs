using AMB.Application.Dtos;
using AMB.Application.Interfaces.Repositories;
using AMB.Application.Interfaces.Services;
using AMB.Application.Mappers;
using AMB.Domain.Enums;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AMB.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IAuthHelper _authHelper;
        private readonly IServiceProvider _serviceProvider;

        public EmployeeService(IEmployeeRepository employeeRepository, IServiceProvider serviceProvider, IAuthHelper authHelper)
        {
            _employeeRepository = employeeRepository;
            _authHelper = authHelper;
            _serviceProvider = serviceProvider;
        }

        public async Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeRequestDto request)
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<CreateEmployeeRequestDto>>();
            await validator.ValidateAndThrowAsync(request);

            var empModel = request.ToEmployeeEntity();

            string? authUserId = null;

            try
            {
                authUserId = await _authHelper.CreateUserAsync(request.Email, request.Password, $"{request.FirstName} {request.LastName}");

                empModel.Status = (int)EntityStatus.Active;
                empModel.UserId = authUserId;
                var emp = await _employeeRepository.AddAsync(empModel);

                return emp.ToEmployeeDto();
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrWhiteSpace(authUserId))
                {
                    await _authHelper.DeleteUserAsync(authUserId);
                }
                
                throw ex;
            }
        }
    }
}
