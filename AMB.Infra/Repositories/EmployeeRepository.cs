using AMB.Application.Interfaces.Repositories;
using AMB.Domain.Entities;
using AMB.Infra.DBContexts;

namespace AMB.Infra.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AMBContext _context;

        public EmployeeRepository(AMBContext context)
        {
            _context = context;
        }

        public async Task<Employee> AddAsync(Employee employee)
        {
            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();
            return employee;
        }
    }
}
