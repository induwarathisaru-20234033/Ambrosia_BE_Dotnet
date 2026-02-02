using AMB.Application.Dtos;
using AMB.Application.Interfaces.Repositories;
using AMB.Application.Interfaces.Services;
using AMB.Application.Mappers;
using AMB.Domain.Enums;
using FluentValidation;

namespace AMB.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IAuthHelper _authHelper;

        private readonly IValidator<CreateEmployeeRequestDto> _validator;

        public EmployeeService(IEmployeeRepository employeeRepository, IValidator<CreateEmployeeRequestDto> validator, IAuthHelper authHelper)
        {
            _employeeRepository = employeeRepository;
            _authHelper = authHelper;
            _validator = validator;
        }

        public async Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeRequestDto request)
        {
            await _validator.ValidateAndThrowAsync(request);

            var empModel = request.ToEmployeeEntity();

            var authUserId = await _authHelper.CreateUserAsync(request.Username, request.Password, $"{request.FirstName} {request.LastName}");

            empModel.Status = (int)EntityStatus.Active;
            empModel.UserId = authUserId;

            var emp = await _employeeRepository.AddAsync(empModel);

            return emp.ToEmployeeDto();
        }


    }
}
