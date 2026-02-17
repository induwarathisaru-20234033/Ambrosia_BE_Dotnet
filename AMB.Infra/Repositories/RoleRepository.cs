using AMB.Application.Interfaces.Repositories;
using AMB.Domain.Entities;
using AMB.Domain.Enums;
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

        public async Task<Role> GetByIdAsync(int id)
        {
            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.Id == id);

            return role ?? throw new KeyNotFoundException($"Role with ID {id} not found");
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

        public async Task<Role> AddAsync(Role role)
        {
            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();
            return role;
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

        public async Task<Role?> GetByIdWithPermissionsAsync(int id)
        {
            return await _context.Roles
                .Include(r => r.RolePermissionMaps!)
                    .ThenInclude(rpm => rpm.Permission!)
                        .ThenInclude(p => p.Feature)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<bool> IsRoleCodeUniqueAsync(string roleCode, int? excludeId = null)
        {
            var query = _context.Roles.Where(r => r.RoleCode == roleCode);

            if (excludeId.HasValue)
            {
                query = query.Where(r => r.Id != excludeId.Value);
            }

            return !await query.AnyAsync();
        }
    }
}
