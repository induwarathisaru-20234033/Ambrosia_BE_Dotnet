using AMB.Application.Dtos;

namespace AMB.Application.Interfaces.Services
{
    public interface IEmployeeService
    {
        Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeRequestDto request);
    }
}
