using AMB.Application.Dtos;
using AMB.Application.Interfaces.Repositories;
using AMB.Application.Interfaces.Services;
using AMB.Application.Mappers;
using AMB.Domain.Enums;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore; //Detuni

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

            var authUserId = await _authHelper.CreateUserAsync(request.Username, request.Password, $"{request.FirstName} {request.LastName}");

            empModel.Status = (int)EntityStatus.Active;
            empModel.UserId = authUserId;

            var emp = await _employeeRepository.AddAsync(empModel);

            return emp.ToEmployeeDto();
        }

        //Detuni
        public async Task<PagedResponseDto<EmployeeDto>> GetEmployeesAsync(EmployeeFilterRequestDto filter)
        {
            var query = _employeeRepository.Query();

            if (!string.IsNullOrEmpty(filter.EmployeeId))
                query = query.Where(e => e.EmployeeId.Contains(filter.EmployeeId));

            if (!string.IsNullOrEmpty(filter.FirstName))
                query = query.Where(e => e.FirstName.Contains(filter.FirstName));

            if (!string.IsNullOrEmpty(filter.LastName))
                query = query.Where(e => e.LastName.Contains(filter.LastName));

            if (!string.IsNullOrEmpty(filter.Username))
                query = query.Where(e => e.Username.Contains(filter.Username));

            if (!string.IsNullOrEmpty(filter.Email))
                query = query.Where(e => e.Email.Contains(filter.Email));

            if (!string.IsNullOrEmpty(filter.MobileNumber))
                query = query.Where(e => e.MobileNumber.Contains(filter.MobileNumber));

            if (!string.IsNullOrEmpty(filter.Address))
                query = query.Where(e => e.Address.Contains(filter.Address));

            //var totalCount = await query.CountAsync();

            //var employees = await query
            //    .Skip((filter.PageNumber - 1) * filter.PageSize)
            //    .Take(filter.PageSize)
            //    .Select(e => e.ToEmployeeDto())
            //    .ToListAsync();

            //return new PagedResponseDto<EmployeeDto>
            //{
            //    Items = employees,
            //    TotalCount = totalCount
            //};

            // get total count for pagination
            var totalCount = await query.CountAsync();

            // fetch actual entities from database
            var employeeEntities = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync(); // ToListAsync works here because these are entities

            // map to DTO in memory
            var employees = employeeEntities
                .Select(e => e.ToEmployeeDto())
                .ToList(); // normal List<T>.ToList(), not async

            // return paged response
            return new PagedResponseDto<EmployeeDto>
            {
                Items = employees,
                TotalCount = totalCount
            };

        }



    }
}
