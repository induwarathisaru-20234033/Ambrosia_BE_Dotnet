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

        //Detuni
        public async Task<PagedResponseDto<EmployeeDto>> GetEmployeesAsync(EmployeeFilterRequestDto filter)
        {
            var query = _employeeRepository.GetSearchQuery();

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

            if (!string.IsNullOrEmpty(filter.Status))
            {
                if (filter.Status.Equals("Active", StringComparison.OrdinalIgnoreCase))
                    query = query.Where(e => e.Status == (int)EntityStatus.Active);
                else if (filter.Status.Equals("Inactive", StringComparison.OrdinalIgnoreCase))
                    query = query.Where(e => e.Status == (int)EntityStatus.Inactive);
            }

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


            var employeeEntities = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            // map to DTO in memory
            var employees = employeeEntities
                .Select(e => e.ToEmployeeDto())
                .ToList(); // normal List<T>.ToList(), not async

            // return paged response
            return new PagedResponseDto<EmployeeDto>
            {
                    Items = employees,
                TotalItemCount = totalCount,
                PageNumber = filter.PageNumber, // current page requested
                PageSize = filter.PageSize,     // items per page
                PageCount = (int)Math.Ceiling((double)totalCount / filter.PageSize) // total pages
            };

        }

        public async Task<EmployeeDto> GetEmployeeByIdAsync(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
            {
                throw new KeyNotFoundException($"Employee with ID {id} not found.");
            }

            return employee.ToEmployeeDto();
        }

        public async Task<EmployeeDto?> UpdateEmployeeAsync(UpdateEmployeeRequestDto request)
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<UpdateEmployeeRequestDto>>();
            await validator.ValidateAndThrowAsync(request);

            var employee = await _employeeRepository.GetByIdAsync(request.Id);
            if (employee == null)
            {
                throw new KeyNotFoundException($"Employee with ID {request.Id} not found.");
            }

            employee.FirstName = request.FirstName;
            employee.LastName = request.LastName;
            employee.Email = request.Email;
            employee.Username = request.Username;
            employee.MobileNumber = request.MobileNumber;
            employee.Address = request.Address;
            employee.Status = (int)request.Status;

            var updatedEmployee = await _employeeRepository.UpdateAsync(employee);

            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                if (string.IsNullOrWhiteSpace(employee.UserId))
                {
                    throw new InvalidOperationException($"Employee with ID {request.Id} does not have an associated Auth0 user ID.");
                }

                await _authHelper.UpdatePasswordAsync(employee.UserId, request.Password);
            }

            return updatedEmployee?.ToEmployeeDto();
        }

        public async Task AssignRolesAsync(AssignEmployeeRolesRequestDto request)
        {
            if (request.EmployeeId <= 0)
            {
                throw new ArgumentException("Valid EmployeeId is required.");
            }

            var roleIds = request.RoleIds?
                .Where(id => id > 0)
                .Distinct()
                .ToList() ?? new List<int>();

            var customRoleIds = request.CustomRoleIds?
                .Where(id => id > 0)
                .Distinct()
                .ToList() ?? new List<int>();

            if (!roleIds.Any() && !customRoleIds.Any())
            {
                throw new ArgumentException("At least one role or custom role id is required.");
            }

            var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId);
            if (employee == null)
            {
                throw new KeyNotFoundException($"Employee with ID {request.EmployeeId} not found.");
            }

            var existingRoleIds = await _employeeRepository.GetExistingRoleIdsAsync(roleIds);
            var missingRoleIds = roleIds.Except(existingRoleIds).ToList();
            if (missingRoleIds.Any())
            {
                throw new KeyNotFoundException($"Invalid role ids: {string.Join(", ", missingRoleIds)}");
            }

            var existingCustomRoleIds = await _employeeRepository.GetExistingCustomRoleIdsAsync(customRoleIds);
            var missingCustomRoleIds = customRoleIds.Except(existingCustomRoleIds).ToList();
            if (missingCustomRoleIds.Any())
            {
                throw new KeyNotFoundException($"Invalid custom role ids: {string.Join(", ", missingCustomRoleIds)}");
            }

            await _employeeRepository.AssignRolesAsync(request.EmployeeId, roleIds, customRoleIds);
        }



    }
}
