using AMB.Application.Dtos;
using AMB.Application.Interfaces.Repositories;
using AMB.Application.Mappers;
using AMB.Domain.Entities;

namespace AMB.Tests.Mocks
{
    internal sealed class TestRoleRepository : IRoleRepository
    {
        public Role? LastAddedRole { get; private set; }
        public CustomRole? LastAddedCustomRole { get; private set; }
        public Role? LastUpdatedRole { get; private set; }
        public List<int>? LastUpdatedPermissionIds { get; private set; }
        public Dictionary<int, Role> Roles { get; } = new();
        public Dictionary<int, CustomRole> CustomRoles { get; } = new();

        public Task<Role> AddAsync(Role role)
        {
            role.Id = Roles.Count + 1;

            // Set IDs for permission maps and ensure they're linked
            if (role.RolePermissionMaps != null)
            {
                foreach (var rpm in role.RolePermissionMaps)
                {
                    rpm.Id = (role.RolePermissionMaps.IndexOf(rpm) + 1) * 100 + role.Id;
                    rpm.RoleId = role.Id;

                    // For test purposes, also set the Permission navigation property
                    if (rpm.PermissionId > 0 && rpm.PermissionId <= 8)
                    {
                        rpm.Permission = new Permission
                        {
                            Id = rpm.PermissionId,
                            PermissionCode = rpm.PermissionId switch
                            {
                                1 => "VIEW_EMP",
                                2 => "CREATE_EMP",
                                3 => "EDIT_EMP",
                                4 => "DELETE_EMP",
                                5 => "VIEW_ROLES",
                                6 => "CREATE_ROLE",
                                7 => "EDIT_ROLE",
                                8 => "DELETE_ROLE",
                                _ => "UNKNOWN"
                            },
                            PermissionName = rpm.PermissionId switch
                            {
                                1 => "View Employees",
                                2 => "Create Employee",
                                3 => "Edit Employee",
                                4 => "Delete Employee",
                                5 => "View Roles",
                                6 => "Create Role",
                                7 => "Edit Role",
                                8 => "Delete Role",
                                _ => "Unknown"
                            },
                            FeatureId = rpm.PermissionId <= 4 ? 1 : 2,
                            Feature = rpm.PermissionId <= 4
                                ? new Feature { Id = 1, FeatureName = "Employee Management", FeatureCode = "EMP_MGMT" }
                                : new Feature { Id = 2, FeatureName = "Role Management", FeatureCode = "ROLE_MGMT" }
                        };
                    }
                }
            }

            LastAddedRole = role;
            Roles[role.Id] = role;
            return Task.FromResult(role);
        }

        public Task<Role> UpdateAsync(Role role)
        {
            Roles[role.Id] = role;
            LastUpdatedRole = role;
            return Task.FromResult(role);
        }

        public Task<CustomRole> AddCustomRoleAsync(CustomRole role)
        {
            role.Id = Roles.Count + 1;

            if (role.CustomRolePermissionMaps != null)
            {
                foreach (var rpm in role.CustomRolePermissionMaps)
                {
                    rpm.Id = (role.CustomRolePermissionMaps.IndexOf(rpm) + 1) * 100 + role.Id;
                    rpm.CustomRoleId = role.Id;

                    if (rpm.PermissionId > 0 && rpm.PermissionId <= 8)
                    {
                        rpm.Permission = new Permission
                        {
                            Id = rpm.PermissionId,
                            PermissionCode = rpm.PermissionId switch
                            {
                                1 => "VIEW_EMP",
                                2 => "CREATE_EMP",
                                3 => "EDIT_EMP",
                                4 => "DELETE_EMP",
                                5 => "VIEW_ROLES",
                                6 => "CREATE_ROLE",
                                7 => "EDIT_ROLE",
                                8 => "DELETE_ROLE",
                                _ => "UNKNOWN"
                            },
                            PermissionName = rpm.PermissionId switch
                            {
                                1 => "View Employees",
                                2 => "Create Employee",
                                3 => "Edit Employee",
                                4 => "Delete Employee",
                                5 => "View Roles",
                                6 => "Create Role",
                                7 => "Edit Role",
                                8 => "Delete Role",
                                _ => "Unknown"
                            },
                            FeatureId = rpm.PermissionId <= 4 ? 1 : 2,
                            Feature = rpm.PermissionId <= 4
                                ? new Feature { Id = 1, FeatureName = "Employee Management", FeatureCode = "EMP_MGMT" }
                                : new Feature { Id = 2, FeatureName = "Role Management", FeatureCode = "ROLE_MGMT" }
                        };
                    }
                }
            }

            LastAddedCustomRole = role;
            CustomRoles[role.Id] = role;

            // Keep existing dictionary behavior for tests that read roles by id via GetByIdAsync.
            Roles[role.Id] = new Role
            {
                Id = role.Id,
                RoleCode = role.RoleCode,
                RoleName = role.RoleName,
                Description = role.Description,
                Status = role.Status,
                RolePermissionMaps = role.CustomRolePermissionMaps?.Select(m => new RolePermissionMap
                {
                    Id = m.Id,
                    RoleId = role.Id,
                    PermissionId = m.PermissionId,
                    Permission = m.Permission
                }).ToList()
            };

            return Task.FromResult(role);
        }

        public Task<CustomRole?> GetCustomRoleByIdAsync(int id, RoleQueryOptions? options = null)
        {
            CustomRoles.TryGetValue(id, out var customRole);
            return Task.FromResult(customRole);
        }

        public Task<Role> UpdateWithPermissionsAsync(Role role, List<int> newPermissionIds)
        {
            // Update role properties
            Roles[role.Id] = role;

            // Create new permission maps with navigation properties
            role.RolePermissionMaps = newPermissionIds.Select((permissionId, index) =>
            {
                var rpm = new RolePermissionMap
                {
                    Id = (index + 1) * 100 + role.Id,
                    RoleId = role.Id,
                    PermissionId = permissionId
                };

                // Set Permission navigation property
                rpm.Permission = new Permission
                {
                    Id = permissionId,
                    PermissionCode = permissionId switch
                    {
                        1 => "VIEW_EMP",
                        2 => "CREATE_EMP",
                        3 => "EDIT_EMP",
                        4 => "DELETE_EMP",
                        5 => "VIEW_ROLES",
                        6 => "CREATE_ROLE",
                        7 => "EDIT_ROLE",
                        8 => "DELETE_ROLE",
                        _ => "UNKNOWN"
                    },
                    PermissionName = permissionId switch
                    {
                        1 => "View Employees",
                        2 => "Create Employee",
                        3 => "Edit Employee",
                        4 => "Delete Employee",
                        5 => "View Roles",
                        6 => "Create Role",
                        7 => "Edit Role",
                        8 => "Delete Role",
                        _ => "Unknown"
                    },
                    FeatureId = permissionId <= 4 ? 1 : 2,
                    Feature = permissionId <= 4
                        ? new Feature { Id = 1, FeatureName = "Employee Management", FeatureCode = "EMP_MGMT" }
                        : new Feature { Id = 2, FeatureName = "Role Management", FeatureCode = "ROLE_MGMT" }
                };

                return rpm;
            }).ToList();

            LastUpdatedRole = role;
            LastUpdatedPermissionIds = newPermissionIds;
            return Task.FromResult(role);
        }

        public Task<Role?> GetByIdAsync(int id, RoleQueryOptions? options = null)
        {
            Roles.TryGetValue(id, out var role);

            // If options request permissions, ensure they're loaded
            if (role != null && options?.IncludePermissions == true && role.RolePermissionMaps != null)
            {
                foreach (var rpm in role.RolePermissionMaps)
                {
                    if (rpm.Permission == null && rpm.PermissionId > 0)
                    {
                        rpm.Permission = new Permission
                        {
                            Id = rpm.PermissionId,
                            PermissionCode = rpm.PermissionId switch
                            {
                                1 => "VIEW_EMP",
                                2 => "CREATE_EMP",
                                3 => "EDIT_EMP",
                                4 => "DELETE_EMP",
                                5 => "VIEW_ROLES",
                                6 => "CREATE_ROLE",
                                7 => "EDIT_ROLE",
                                8 => "DELETE_ROLE",
                                _ => "UNKNOWN"
                            },
                            PermissionName = rpm.PermissionId switch
                            {
                                1 => "View Employees",
                                2 => "Create Employee",
                                3 => "Edit Employee",
                                4 => "Delete Employee",
                                5 => "View Roles",
                                6 => "Create Role",
                                7 => "Edit Role",
                                8 => "Delete Role",
                                _ => "Unknown"
                            },
                            FeatureId = rpm.PermissionId <= 4 ? 1 : 2
                        };

                        if (options.IncludePermissionFeatures)
                        {
                            rpm.Permission.Feature = rpm.PermissionId <= 4
                                ? new Feature { Id = 1, FeatureName = "Employee Management", FeatureCode = "EMP_MGMT" }
                                : new Feature { Id = 2, FeatureName = "Role Management", FeatureCode = "ROLE_MGMT" };
                        }
                    }
                }
            }

            return Task.FromResult(role);
        }

        public Task<Role> GetByRoleCodeAsync(string roleCode)
        {
            var role = Roles.Values.FirstOrDefault(r => r.RoleCode == roleCode);
            return Task.FromResult(role!);
        }

        public Task<List<Role>> GetAllAsync()
        {
            return Task.FromResult(Roles.Values.ToList());
        }

        public Task<PaginatedResultDto<RoleDto>> GetAllRolesAsync(RoleFilterRequestDto filter)
        {
            // Get all system roles as DTOs
            var systemRoles = Roles.Values.Select(r => r.ToRoleDto()).ToList();

            // Get all custom roles as DTOs
            var customRoles = CustomRoles.Values.Select(r => r.ToRoleDto()).ToList();

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

            return Task.FromResult(new PaginatedResultDto<RoleDto>
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                PageCount = pageCount,
                TotalItemCount = totalCount,
                Items = pagedRoles
            });
        }

        public Task<Role> DeleteAsync(Role role)
        {
            Roles.Remove(role.Id);
            return Task.FromResult(role);
        }

        public Task<bool> IsRoleCodeUniqueAsync(string roleCode, int? excludeId = null)
        {
            var existsInRoles = Roles.Values.Any(r =>
                r.RoleCode == roleCode &&
                (!excludeId.HasValue || r.Id != excludeId.Value));

            var existsInCustomRoles = CustomRoles.Values.Any(r =>
                r.RoleCode == roleCode &&
                (!excludeId.HasValue || r.Id != excludeId.Value));

            return Task.FromResult(!existsInRoles && !existsInCustomRoles);
        }
    }
}