using AMB.Application.Dtos;

namespace AMB.Application.Interfaces.Services
{
    public interface IEmployeeService
    {
        Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeRequestDto request);
        //Detuni
        Task<PagedResponseDto<EmployeeDto>> GetEmployeesAsync(EmployeeFilterRequestDto filter);
        Task<EmployeeDto> GetEmployeeByIdAsync(int id);
        Task<EmployeeDto?> UpdateEmployeeAsync(UpdateEmployeeRequestDto request);
        Task<EmployeeDto> UpdateEmployeeOnlineStatusAsync(UpdateEmployeeOnlineStatusRequestDto request);
        Task<List<WaiterAllocationDto>> GetWaitersWithCurrentAllocationsAsync();
        Task AssignRolesAsync(AssignEmployeeRolesRequestDto request);

    }
}
