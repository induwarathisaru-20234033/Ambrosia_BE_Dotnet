using AMB.Application.Interfaces.Repositories;
using AMB.Domain.Entities;

namespace AMB.Tests.Mocks
{
    internal sealed class TestPermissionRepository : IPermissionRepository
    {
        private readonly List<Permission> _permissions = new();
        private readonly List<Feature> _features = new();

        public TestPermissionRepository()
        {
            // Setup test features
            _features.AddRange(new[]
            {
                new Feature { Id = 1, FeatureName = "Employee Management", FeatureCode = "EMP_MGMT" },
                new Feature { Id = 2, FeatureName = "Role Management", FeatureCode = "ROLE_MGMT" }
            });

            // Setup test permissions
            _permissions.AddRange(new[]
            {
                new Permission { Id = 1, PermissionCode = "VIEW_EMP", PermissionName = "View Employees", FeatureId = 1, Feature = _features[0] },
                new Permission { Id = 2, PermissionCode = "CREATE_EMP", PermissionName = "Create Employee", FeatureId = 1, Feature = _features[0] },
                new Permission { Id = 3, PermissionCode = "EDIT_EMP", PermissionName = "Edit Employee", FeatureId = 1, Feature = _features[0] },
                new Permission { Id = 4, PermissionCode = "DELETE_EMP", PermissionName = "Delete Employee", FeatureId = 1, Feature = _features[0] },
                new Permission { Id = 5, PermissionCode = "VIEW_ROLES", PermissionName = "View Roles", FeatureId = 2, Feature = _features[1] },
                new Permission { Id = 6, PermissionCode = "CREATE_ROLE", PermissionName = "Create Role", FeatureId = 2, Feature = _features[1] },
                new Permission { Id = 7, PermissionCode = "EDIT_ROLE", PermissionName = "Edit Role", FeatureId = 2, Feature = _features[1] },
                new Permission { Id = 8, PermissionCode = "DELETE_ROLE", PermissionName = "Delete Role", FeatureId = 2, Feature = _features[1] }
            });
        }

        public Task<Permission> GetByIdAsync(int id)
        {
            var permission = _permissions.First(p => p.Id == id);
            return Task.FromResult(permission);
        }

        public Task<List<Permission>> GetAllAsync()
        {
            return Task.FromResult(_permissions.ToList());
        }

        public Task<List<Permission>> GetByIdsAsync(List<int> ids)
        {
            var permissions = _permissions.Where(p => ids.Contains(p.Id)).ToList();
            return Task.FromResult(permissions);
        }

        public Task<List<Permission>> GetPermissionsWithFeaturesAsync()
        {
            return Task.FromResult(_permissions.ToList());
        }
    }
}