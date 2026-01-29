using AMB.Domain.Entities;

namespace AMB.Application.Interfaces.Repositories
{
    public interface IEmployeeRepository
    {
        Task<Employee> AddAsync(Employee employee);

        Task<Employee> GetByUserIDAsync(string userId);
    }
}
