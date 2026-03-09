using AMB.Application.Interfaces.Repositories;
using AMB.Domain.Entities;

namespace AMB.Tests.Mocks
{
    internal sealed class TestEmployeeRepository : IEmployeeRepository
    {
        public Employee? LastAddedEmployee { get; private set; }
        public Employee? EmployeeByUsername { get; set; }
        public Dictionary<int, Employee> Employees { get; } = new();
        public List<EmployeeRoleMap> EmployeeRoleMaps { get; } = new();

        public Task<Employee> AddAsync(Employee employee)
        {
            employee.Id = 42;
            LastAddedEmployee = employee;
            Employees[employee.Id] = employee;
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

        public Task<Employee?> GetByIdAsync(int id)
        {
            Employees.TryGetValue(id, out var employee);
            return Task.FromResult(employee);
        }

        public Task<List<int>> GetExistingRoleIdsAsync(List<int> roleIds)
        {
            // Assume all provided role ids exist in test defaults.
            return Task.FromResult(roleIds.ToList());
        }

        public Task<List<int>> GetExistingCustomRoleIdsAsync(List<int> customRoleIds)
        {
            // Assume all provided custom role ids exist in test defaults.
            return Task.FromResult(customRoleIds.ToList());
        }

        public Task AssignRolesAsync(int employeeId, List<int> roleIds, List<int> customRoleIds)
        {
            EmployeeRoleMaps.RemoveAll(x => x.EmployeeId == employeeId);

            EmployeeRoleMaps.AddRange(roleIds.Select(roleId => new EmployeeRoleMap
            {
                EmployeeId = employeeId,
                RoleId = roleId,
                CustomRoleId = null,
                Status = 1
            }));

            EmployeeRoleMaps.AddRange(customRoleIds.Select(customRoleId => new EmployeeRoleMap
            {
                EmployeeId = employeeId,
                RoleId = null,
                CustomRoleId = customRoleId,
                Status = 1
            }));

            return Task.CompletedTask;
        }

        // <-- Implement the missing interface method
        public IQueryable<Employee> GetSearchQuery()
        {
            var employees = new List<Employee>();
            return employees.AsQueryable();
        }
    }
}
