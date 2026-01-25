using AMB.Application.Dtos;
using AMB.Application.Interfaces.Repositories;
using AMB.Application.Interfaces.Services;
using AMB.Application.Mappers;
using AMB.Domain.Enums;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AMB.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        private readonly IValidator<CreateEmployeeRequestDto> _validator;

        public EmployeeService(IEmployeeRepository employeeRepository, IValidator<CreateEmployeeRequestDto> validator)
        {
            _employeeRepository = employeeRepository;
            _validator = validator;
        }

        public async Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeRequestDto request)
        {
            try
            {
                await _validator.ValidateAndThrowAsync(request);

                var empModel = request.ToEmployeeEntity();

                empModel.Status = (int)EntityStatus.Active;

                var emp = await _employeeRepository.AddAsync(empModel);

                return emp.ToEmployeeDto();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
