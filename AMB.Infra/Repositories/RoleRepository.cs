using AMB.Application.Dtos;
using AMB.Application.Interfaces.Repositories;
using AMB.Application.Mappers;
using AMB.Domain.Entities;
using AMB.Infra.DBContexts;
using Microsoft.EntityFrameworkCore;

namespace AMB.Infra.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AMBContext _context;

        public RoleRepository(AMBContext context)
        {
            _context = context;
        }

        public async Task<Role?> GetByIdAsync(int id, RoleQueryOptions? options = null)
        {
            options ??= new RoleQueryOptions();

            IQueryable<Role> query = _context.Roles;

            if (options.IncludePermissions)
            {
                query = query.Include(r => r.RolePermissionMaps!);

                if (options.IncludePermissionFeatures)
                {
                    query = query.Include(r => r.RolePermissionMaps!)
                        .ThenInclude(rpm => rpm.Permission!)
                        .ThenInclude(p => p.Feature);
                }
            }

            var role = await query.FirstOrDefaultAsync(r => r.Id == id);

            return role;
        }

        public async Task<Role> GetByRoleCodeAsync(string roleCode)
        {
            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleCode == roleCode);

            return role ?? throw new KeyNotFoundException($"Role with code {roleCode} not found");
        }

        public async Task<List<Role>> GetAllAsync()
        {
            return await _context.Roles
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();
        }

        public async Task<PaginatedResultDto<RoleDto>> GetAllRolesAsync(RoleFilterRequestDto filter)
        {
            // Get all system roles
            var systemRoles = await _context.Roles
                .Select(r => r.ToRoleDto())
                .ToListAsync();

            // Get all custom roles
            var customRoles = await _context.CustomRoles
                .Select(r => r.ToRoleDto())
                .ToListAsync();

            // Concatenate both lists
            var allRoles = systemRoles.Concat(customRoles)
                .OrderByDescending(r => r.CreatedDate)
                .ToList();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(filter.RoleName))
            {
                allRoles = allRoles.Where(r =>
                    r.Name.Contains(filter.RoleName, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(filter.Description))
            {
                allRoles = allRoles.Where(r =>
                    !string.IsNullOrEmpty(r.Description) &&
                    r.Description.Contains(filter.Description, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Calculate pagination
            var totalCount = allRoles.Count;
            var pageCount = (int)Math.Ceiling(totalCount / (double)filter.PageSize);

            // Apply pagination
            var pagedRoles = allRoles
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            return new PaginatedResultDto<RoleDto>
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                PageCount = pageCount,
                TotalItemCount = totalCount,
                Items = pagedRoles
            };
        }

        public async Task<Role> AddAsync(Role role)
        {
            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();
            return role;
        }

        public async Task<CustomRole> AddCustomRoleAsync(CustomRole role)
        {
            await _context.CustomRoles.AddAsync(role);
            await _context.SaveChangesAsync();
            return role;
        }

        public async Task<CustomRole?> GetCustomRoleByIdAsync(int id, RoleQueryOptions? options = null)
        {
            options ??= new RoleQueryOptions();

            IQueryable<CustomRole> query = _context.CustomRoles;

            if (options.IncludePermissions)
            {
                query = query.Include(r => r.CustomRolePermissionMaps!);

                if (options.IncludePermissionFeatures)
                {
                    query = query
                        .Include(r => r.CustomRolePermissionMaps!)
                        .ThenInclude(rpm => rpm.Permission!)
                        .ThenInclude(p => p.Feature);
                }
            }

            return await query.FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Role> UpdateAsync(Role role)
        {
            _context.Roles.Update(role);
            await _context.SaveChangesAsync();
            return role;
        }

        public async Task<Role> DeleteAsync(Role role)
        {
            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
            return role;
        }


        public async Task<bool> IsRoleCodeUniqueAsync(string roleCode, int? excludeId = null)
        {
            var query = _context.Roles.Where(r => r.RoleCode == roleCode);
            var customRoleQuery = _context.CustomRoles.Where(r => r.RoleCode == roleCode);

            if (excludeId.HasValue)
            {
                query = query.Where(r => r.Id != excludeId.Value);
                customRoleQuery = customRoleQuery.Where(r => r.Id != excludeId.Value);
            }

            return !await query.AnyAsync() && !await customRoleQuery.AnyAsync();
        }


        //Update role with permissions
        public async Task<Role> UpdateWithPermissionsAsync(Role role, List<int> newPermissionIds)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Roles.Update(role);

                var existingMaps = _context.RolePermissionMaps
                    .Where(rpm => rpm.RoleId == role.Id);
                _context.RolePermissionMaps.RemoveRange(existingMaps);

                var newMaps = newPermissionIds.Select(permissionId => new RolePermissionMap
                {
                    RoleId = role.Id,
                    PermissionId = permissionId
                });
                await _context.RolePermissionMaps.AddRangeAsync(newMaps);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return role;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<RoleAssignedEmployeesDto?> GetAssignedEmployeesByRoleAsync(int roleId, bool isCustomRole = false)
        {
            if (isCustomRole)
            {
                var customRole = await _context.CustomRoles
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r => r.Id == roleId);

                if (customRole == null)
                {
                    return null;
                }

                var assignedEmployees = await _context.EmployeeRoleMaps
                    .Where(erm => erm.CustomRoleId == roleId)
                    .Include(erm => erm.Employee)
                    .Where(erm => erm.Employee != null)
                    .Select(erm => new AssignedRoleEmployeeDto
                    {
                        Id = erm.Employee!.Id,
                        EmployeeId = erm.Employee.EmployeeId,
                        FirstName = erm.Employee.FirstName,
                        LastName = erm.Employee.LastName,
                        Username = erm.Employee.Username,
                        Email = erm.Employee.Email,
                        MobileNumber = erm.Employee.MobileNumber,
                        IsOnline = erm.Employee.IsOnline,
                        Status = erm.Employee.Status
                    })
                    .Distinct()
                    .ToListAsync();

                return new RoleAssignedEmployeesDto
                {
                    RoleId = customRole.Id,
                    RoleCode = customRole.RoleCode,
                    RoleName = customRole.RoleName,
                    IsCustomRole = true,
                    AssignedEmployees = assignedEmployees
                };
            }

            var role = await _context.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == roleId);

            if (role == null)
            {
                return null;
            }

            var employees = await _context.EmployeeRoleMaps
                .Where(erm => erm.RoleId == roleId)
                .Include(erm => erm.Employee)
                .Where(erm => erm.Employee != null)
                .Select(erm => new AssignedRoleEmployeeDto
                {
                    Id = erm.Employee!.Id,
                    EmployeeId = erm.Employee.EmployeeId,
                    FirstName = erm.Employee.FirstName,
                    LastName = erm.Employee.LastName,
                    Username = erm.Employee.Username,
                    Email = erm.Employee.Email,
                    MobileNumber = erm.Employee.MobileNumber,
                    IsOnline = erm.Employee.IsOnline,
                    Status = erm.Employee.Status
                })
                .Distinct()
                .ToListAsync();

            return new RoleAssignedEmployeesDto
            {
                RoleId = role.Id,
                RoleCode = role.RoleCode,
                RoleName = role.RoleName,
                IsCustomRole = false,
                AssignedEmployees = employees
            };
        }
    }
}
