using AMB.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;

namespace AMB.Infra.DBContexts
{
    public class AMBContext: DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AMBContext> _logger;

        public AMBContext(DbContextOptions<AMBContext> options, IHttpContextAccessor httpContextAccessor, ILogger<AMBContext> logger) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Feature> Features { get; set; }
        public DbSet<RolePermissionMap> RolePermissionMaps { get; set; }
        public DbSet<EmployeeRoleMap> EmployeeRoleMaps { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().ToTable(nameof(Employees));
            modelBuilder.Entity<Role>().ToTable(nameof(Roles));
            modelBuilder.Entity<Permission>().ToTable(nameof(Permissions));
            modelBuilder.Entity<Feature>().ToTable(nameof(Features));
            modelBuilder.Entity<RolePermissionMap>().ToTable(nameof(RolePermissionMaps));
            modelBuilder.Entity<EmployeeRoleMap>().ToTable(nameof(EmployeeRoleMaps));

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.EmployeeId)
                .IsUnique()
                .HasFilter("[Status] = 1");

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.MobileNumber)
                .IsUnique()
                .HasFilter("[Status] = 1");

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.Username)
                .IsUnique()
                .HasFilter("[Status] = 1");

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.UserId)
                .IsUnique()
                .HasFilter("[Status] = 1");

            modelBuilder.Entity<Permission>()
                .HasOne(p => p.Feature)
                .WithMany(f => f.Permissions)
                .HasForeignKey(p => p.FeatureId);

            modelBuilder.Entity<RolePermissionMap>()
                .HasOne(rpm => rpm.Role)
                .WithMany(r => r.RolePermissionMaps)
                .HasForeignKey(rpm => rpm.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RolePermissionMap>()
                .HasOne(rpm => rpm.Permission)
                .WithMany(p => p.RolePermissionMaps)
                .HasForeignKey(rpm => rpm.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EmployeeRoleMap>()
                .HasOne(erm => erm.Employee)
                .WithMany(e => e.EmployeeRoleMaps)
                .HasForeignKey(erm => erm.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EmployeeRoleMap>()
                .HasOne(erm => erm.Role)
                .WithMany(r => r.EmployeeRoleMaps)
                .HasForeignKey(erm => erm.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

        }
 
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var entries = ChangeTracker.Entries()
                    .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

                string currentUser = await GetCurrentUser();

                foreach (var entry in entries)
                {
                    if (entry.Entity is BaseEntity entity)
                    {
                        if (entry.State == EntityState.Added)
                        {
                            entity.CreatedBy = !string.IsNullOrEmpty(entity.CreatedBy) ? entity.CreatedBy : currentUser;
                            entity.UpdatedBy = !string.IsNullOrEmpty(entity.UpdatedBy) ? entity.UpdatedBy : currentUser;
                            entity.CreatedDate = DateTimeOffset.UtcNow;
                        }
                        else
                        {
                            entity.UpdatedBy = currentUser;
                        }

                        entity.UpdatedDate = DateTimeOffset.UtcNow;
                    }
                }

                return await base.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task<string> GetCurrentUser()
        {
            var context = _httpContextAccessor.HttpContext;
            var accessToken = context?.Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");

            string currentUser = "SYSTEM";

            if (!string.IsNullOrEmpty(accessToken))
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(accessToken) as JwtSecurityToken;

                if (jsonToken != null)
                {
                    var sub = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "sub")?.Value;
                    var emp = await Employees.FirstOrDefaultAsync(e => e.UserId == sub);
                    currentUser = emp?.Email ?? "SYSTEM";
                }
            }

            return currentUser;
        }
    }
}
