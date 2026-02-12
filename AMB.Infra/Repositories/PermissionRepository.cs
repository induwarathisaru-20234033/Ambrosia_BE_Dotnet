using AMB.Application.Dtos;
using AMB.Application.Interfaces.Repositories;
using AMB.Domain.Entities;
using AMB.Infra.DBContexts;
using Microsoft.EntityFrameworkCore;

namespace AMB.Infra.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly AMBContext _context;

        public PermissionRepository(AMBContext context)
        {
            _context = context;
        }

        public async Task<Permission> GetByIdAsync(int id)
        {
            var permission = await _context.Permissions
                .Include(p => p.Feature)
                .FirstOrDefaultAsync(p => p.Id == id);

            return permission ?? throw new KeyNotFoundException($"Permission with ID {id} not found");
        }

        public async Task<List<Permission>> GetAllAsync()
        {
            return await _context.Permissions
                .Include(p => p.Feature)
                .OrderBy(p => p.FeatureId)
                .ThenBy(p => p.PermissionName)
                .ToListAsync();
        }

        public async Task<List<Permission>> GetByIdsAsync(List<int> ids)
        {
            return await _context.Permissions
                .Include(p => p.Feature)
                .Where(p => ids.Contains(p.Id))
                .OrderBy(p => p.FeatureId)
                .ThenBy(p => p.PermissionName)
                .ToListAsync();
        }

        public async Task<List<PermissionGroupDto>> GetPermissionsGroupedByFeatureAsync()
        {
            var permissions = await _context.Permissions
                .Include(p => p.Feature)
                .OrderBy(p => p.FeatureId)
                .ThenBy(p => p.PermissionName)
                .ToListAsync();

            return permissions
                .GroupBy(p => new { p.FeatureId, p.Feature!.FeatureName, p.Feature.FeatureCode })
                .Select(g => new PermissionGroupDto
                {
                    FeatureId = g.Key.FeatureId,
                    FeatureName = g.Key.FeatureName,
                    FeatureCode = g.Key.FeatureCode,
                    Permissions = g.Select(p => new PermissionItemDto
                    {
                        Id = p.Id,
                        PermissionCode = p.PermissionCode,
                        Name = p.PermissionName,
                        Description = p.PermissionName 
                    }).ToList()
                })
                .ToList();
        }
    }
}
