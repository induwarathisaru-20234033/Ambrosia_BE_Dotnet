using AMB.Application.Interfaces.Repositories;
using AMB.Domain.Entities;
using AMB.Infra.DBContexts;
using Microsoft.EntityFrameworkCore;

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

        public async Task<Employee?> GetByUserIDAsync(string userId)
        {
            return await _context.Employees
                .Include(e => e.EmployeeRoleMaps)
                .ThenInclude(erm => erm.Role)
                .ThenInclude(r => r.RolePermissionMaps)
                .ThenInclude(rpm => rpm.Permission)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.UserId == userId);
        }

        //Detuni
        public IQueryable<Employee> GetSearchQuery()
        {
            return _context.Employees.AsNoTracking();
        }

    }
}
