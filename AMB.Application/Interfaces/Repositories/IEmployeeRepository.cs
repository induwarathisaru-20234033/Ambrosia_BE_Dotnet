using AMB.Domain.Entities;

namespace AMB.Application.Interfaces.Repositories
{
    public interface IEmployeeRepository
    {
        Task<Employee> AddAsync(Employee employee);

        Task<Employee> GetByUserIDAsync(string userId);

        Task<Employee?> GetByUsernameAsync(string username);
      
        IQueryable<Employee> GetSearchQuery();  //Detuni

    }
}
