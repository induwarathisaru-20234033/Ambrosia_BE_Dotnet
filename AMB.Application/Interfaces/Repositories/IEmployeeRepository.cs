using AMB.Domain.Entities;

namespace AMB.Application.Interfaces.Repositories
{
    public interface IEmployeeRepository
    {
        Task<Employee> AddAsync(Employee employee);

        Task<Employee> GetByUserIDAsync(string userId);

        Task<Employee?> GetByUsernameAsync(string username);
        Task<Employee?> GetByIdAsync(int id);
        Task<Employee?> UpdateAsync(Employee employee);
        Task<Employee?> UpdateOnlineStatusAsync(int id, bool isOnline);
        Task<List<Employee>> GetActiveWaitersAsync();
        Task<List<int>> GetExistingRoleIdsAsync(List<int> roleIds);
        Task<List<int>> GetExistingCustomRoleIdsAsync(List<int> customRoleIds);
        Task AssignRolesAsync(int employeeId, List<int> roleIds, List<int> customRoleIds);

        IQueryable<Employee> GetSearchQuery();  //Detuni

    }
}
