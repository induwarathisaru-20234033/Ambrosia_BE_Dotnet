using AMB.Application.Interfaces.Repositories;
using AMB.Domain.Entities;
using AMB.Domain.Enums;
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
                .Include(e => e.EmployeeRoleMaps)
                .ThenInclude(erm => erm.CustomRole)
                .ThenInclude(cr => cr.CustomRolePermissionMaps)
                .ThenInclude(crpm => crpm.Permission)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.UserId == userId);
        }

        public async Task<Employee?> GetByUsernameAsync(string username)
        {
            return await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Username == username && e.Status == (int)EntityStatus.Active);
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            return await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Employee?> UpdateAsync(Employee employee)
        {
            var existingEmployee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == employee.Id);
            if (existingEmployee == null)
            {
                return null;
            }

            existingEmployee.FirstName = employee.FirstName;
            existingEmployee.LastName = employee.LastName;
            existingEmployee.Email = employee.Email;
            existingEmployee.Username = employee.Username;
            existingEmployee.MobileNumber = employee.MobileNumber;
            existingEmployee.Address = employee.Address;
            existingEmployee.Status = employee.Status;
            existingEmployee.IsOnline = employee.IsOnline;

            _context.Employees.Update(existingEmployee);
            await _context.SaveChangesAsync();
            return existingEmployee;
        }

        public async Task<Employee?> UpdateOnlineStatusAsync(int id, bool isOnline)
        {
            var existingEmployee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id);
            if (existingEmployee == null)
            {
                return null;
            }

            existingEmployee.IsOnline = isOnline;

            _context.Employees.Update(existingEmployee);
            await _context.SaveChangesAsync();
            return existingEmployee;
        }

        public async Task<List<Employee>> GetActiveWaitersAsync()
        {
            return await _context.Employees
                .Include(e => e.EmployeeRoleMaps!)
                .ThenInclude(erm => erm.Role)
                .Where(e => e.Status == (int)EntityStatus.Active &&
                            e.EmployeeRoleMaps != null &&
                            e.EmployeeRoleMaps.Any(map =>
                                map.Role != null &&
                                map.Role.RoleName == AMB.Domain.Constants.Role.WaiterRole))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<int>> GetExistingRoleIdsAsync(List<int> roleIds)
        {
            if (!roleIds.Any())
            {
                return new List<int>();
            }

            return await _context.Roles
                .Where(r => roleIds.Contains(r.Id))
                .Select(r => r.Id)
                .ToListAsync();
        }

        public async Task<List<int>> GetExistingCustomRoleIdsAsync(List<int> customRoleIds)
        {
            if (!customRoleIds.Any())
            {
                return new List<int>();
            }

            return await _context.CustomRoles
                .Where(r => customRoleIds.Contains(r.Id))
                .Select(r => r.Id)
                .ToListAsync();
        }

        public async Task AssignRolesAsync(int employeeId, List<int> roleIds, List<int> customRoleIds)
        {
            var existingMaps = _context.EmployeeRoleMaps.Where(x => x.EmployeeId == employeeId);
            _context.EmployeeRoleMaps.RemoveRange(existingMaps);

            var systemRoleMaps = roleIds.Select(roleId => new EmployeeRoleMap
            {
                EmployeeId = employeeId,
                RoleId = roleId,
                CustomRoleId = null,
                Status = (int)EntityStatus.Active
            });

            var customRoleMaps = customRoleIds.Select(customRoleId => new EmployeeRoleMap
            {
                EmployeeId = employeeId,
                RoleId = null,
                CustomRoleId = customRoleId,
                Status = (int)EntityStatus.Active
            });

            await _context.EmployeeRoleMaps.AddRangeAsync(systemRoleMaps.Concat(customRoleMaps));
            await _context.SaveChangesAsync();
        }

        //Detuni
        public IQueryable<Employee> GetSearchQuery()
        {
            return _context.Employees.AsNoTracking();
        }

    }
}
