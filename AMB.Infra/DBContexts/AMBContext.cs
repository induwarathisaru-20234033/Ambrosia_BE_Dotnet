using AMB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AMB.Infra.DBContexts
{
    public class AMBContext: DbContext
    {
        private readonly ILogger<AMBContext> _logger;

        public AMBContext(DbContextOptions<AMBContext> options, ILogger<AMBContext> logger) : base(options)
        {
            _logger = logger;
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Feature> Features { get; set; }
        public DbSet<RolePermissionMap> RolePermissionMaps { get; set; }
        public DbSet<EmployeeRoleMap> EmployeeRoleMaps { get; set; }
        public DbSet<ReservationSetting> ReservationSettings { get; set; }
        public DbSet<ServiceHour> ServiceHours { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<CalenderExclusion> CalenderExclusions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().ToTable(nameof(Employees));
            modelBuilder.Entity<Role>().ToTable(nameof(Roles));
            modelBuilder.Entity<Permission>().ToTable(nameof(Permissions));
            modelBuilder.Entity<Feature>().ToTable(nameof(Features));
            modelBuilder.Entity<RolePermissionMap>().ToTable(nameof(RolePermissionMaps));
            modelBuilder.Entity<EmployeeRoleMap>().ToTable(nameof(EmployeeRoleMaps));
            modelBuilder.Entity<ReservationSetting>().ToTable(nameof(ReservationSettings));
            modelBuilder.Entity<ServiceHour>().ToTable(nameof(ServiceHours));
            modelBuilder.Entity<Table>().ToTable(nameof(Tables));
            modelBuilder.Entity<CalenderExclusion>().ToTable(nameof(CalenderExclusions));


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

        // Update this method to get the current user from access token 
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var entries = ChangeTracker.Entries()
                    .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

                string currentUser = "SYSTEM";

                foreach (var entry in entries)
                {
                    if (entry.Entity is BaseEntity entity)
                    {
                        if (entry.State == EntityState.Added)
                        {
                            entity.CreatedBy = !string.IsNullOrEmpty(entity.CreatedBy) ? entity.CreatedBy : currentUser;
                            entity.UpdatedBy = !string.IsNullOrEmpty(entity.UpdatedBy) ? entity.UpdatedBy : currentUser;
                            entity.CreatedDate = DateTime.UtcNow;
                        }
                        else
                        {
                            entity.UpdatedBy = currentUser;
                        }

                        entity.UpdatedDate = DateTime.UtcNow;
                    }
                }

                return await base.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
