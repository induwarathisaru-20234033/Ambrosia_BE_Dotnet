using AMB.Application.Interfaces.Repositories;
using AMB.Domain.Entities;

namespace AMB.Tests.Mocks
{
    internal sealed class TestEmployeeRepository : IEmployeeRepository
    {
        public Employee? LastAddedEmployee { get; private set; }
        public Employee? EmployeeByUsername { get; set; }

        public Task<Employee> AddAsync(Employee employee)
        {
            employee.Id = 42;
            LastAddedEmployee = employee;
            return Task.FromResult(employee);
        }

        public Task<Employee?> GetByUserIDAsync(string userId)
        {
            return Task.FromResult<Employee?>(null);
        }

        public Task<Employee?> GetByUsernameAsync(string username)
        {
            return Task.FromResult(EmployeeByUsername);
        }
        
    // <-- Implement the missing interface method
        public IQueryable<Employee> GetSearchQuery()
        {
            var employees = new List<Employee>();
            return employees.AsQueryable();
        }
    }
}
