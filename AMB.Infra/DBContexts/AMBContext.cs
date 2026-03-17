using AMB.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;

namespace AMB.Infra.DBContexts
{
    public class AMBContext : DbContext
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
        public DbSet<CustomRole> CustomRoles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Feature> Features { get; set; }
        public DbSet<RolePermissionMap> RolePermissionMaps { get; set; }
        public DbSet<CustomRolePermissionMap> CustomRolePermissionMaps { get; set; }
        public DbSet<EmployeeRoleMap> EmployeeRoleMaps { get; set; }
        public DbSet<ReservationSetting> ReservationSettings { get; set; }
        public DbSet<ServiceHour> ServiceHours { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<CalenderExclusion> CalenderExclusions { get; set; }

        // Menu Item
        public DbSet<MenuItem> MenuItems { get; set; }

        //Reservation Entities
        public DbSet<BookingSlot> BookingSlots { get; set; }
        public DbSet<CustomerDetail> CustomerDetails { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<UnitOfMeasure> UnitsOfMeasure { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<PurchaseRequest> PurchaseRequests { get; set; }
        public DbSet<PurchaseRequestItem> PurchaseRequestItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().ToTable(nameof(Employees));
            modelBuilder.Entity<Role>().ToTable(nameof(Roles));
            modelBuilder.Entity<CustomRole>().ToTable(nameof(CustomRoles));
            modelBuilder.Entity<Permission>().ToTable(nameof(Permissions));
            modelBuilder.Entity<Feature>().ToTable(nameof(Features));
            modelBuilder.Entity<RolePermissionMap>().ToTable(nameof(RolePermissionMaps));
            modelBuilder.Entity<CustomRolePermissionMap>().ToTable(nameof(CustomRolePermissionMaps));
            modelBuilder.Entity<EmployeeRoleMap>().ToTable(nameof(EmployeeRoleMaps));
            modelBuilder.Entity<ReservationSetting>().ToTable(nameof(ReservationSettings));
            modelBuilder.Entity<ServiceHour>().ToTable(nameof(ServiceHours));
            modelBuilder.Entity<Table>().ToTable(nameof(Tables));
            modelBuilder.Entity<CalenderExclusion>().ToTable(nameof(CalenderExclusions));

            modelBuilder.Entity<MenuItem>().ToTable(nameof(MenuItems));

            modelBuilder.Entity<BookingSlot>().ToTable(nameof(BookingSlots));
            modelBuilder.Entity<CustomerDetail>().ToTable(nameof(CustomerDetails));
            modelBuilder.Entity<Reservation>().ToTable(nameof(Reservations));
            modelBuilder.Entity<InventoryItem>().ToTable(nameof(InventoryItems));
            modelBuilder.Entity<UnitOfMeasure>().ToTable(nameof(UnitsOfMeasure));
            modelBuilder.Entity<Currency>().ToTable(nameof(Currencies));
            modelBuilder.Entity<PurchaseRequest>().ToTable(nameof(PurchaseRequests));
            modelBuilder.Entity<PurchaseRequestItem>().ToTable(nameof(PurchaseRequestItems));

            // Add Order tables
            modelBuilder.Entity<Order>().ToTable(nameof(Orders));
            modelBuilder.Entity<OrderItem>().ToTable(nameof(OrderItems));

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.EmployeeId)
                .IsUnique()
                .HasFilter("[Status] = 1")
                .HasDatabaseName("UX_Employees_EmployeeId_Active");

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.MobileNumber)
                .IsUnique()
                .HasFilter("[Status] = 1")
                .HasDatabaseName("UX_Employees_MobileNumber_Active");

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.Username)
                .IsUnique()
                .HasFilter("[Status] = 1")
                .HasDatabaseName("UX_Employees_Username_Active");

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.UserId)
                .IsUnique()
                .HasFilter("[Status] = 1")
                .HasDatabaseName("UX_Employees_UserId_Active");

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

            modelBuilder.Entity<CustomRolePermissionMap>()
                .HasOne(crpm => crpm.CustomRole)
                .WithMany(cr => cr.CustomRolePermissionMaps)
                .HasForeignKey(crpm => crpm.CustomRoleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CustomRolePermissionMap>()
                .HasOne(crpm => crpm.Permission)
                .WithMany()
                .HasForeignKey(crpm => crpm.PermissionId)
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
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmployeeRoleMap>()
                .HasOne(erm => erm.CustomRole)
                .WithMany()
                .HasForeignKey(erm => erm.CustomRoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MenuItem>()
                .Property(m => m.Price)
                .HasPrecision(18, 2);

            // Reservation relationships
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.CustomerDetail)
                .WithMany()
                .HasForeignKey(r => r.CustomerDetailId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.BookingSlot)
                .WithMany()
                .HasForeignKey(r => r.BookingSlotId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Table)
                .WithMany()
                .HasForeignKey(r => r.TableId)
                .OnDelete(DeleteBehavior.Restrict);

            // Reservation configuration
            modelBuilder.Entity<Reservation>()
                .HasIndex(r => r.ReservationCode)
                .IsUnique();

            modelBuilder.Entity<Reservation>()
                .Property(r => r.ReservationCode)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Reservation>()
                .Property(r => r.PartySize)
                .IsRequired();

            modelBuilder.Entity<Reservation>()
                .Property(r => r.ReservationStatus)
                .IsRequired();

            // CustomerDetail configuration
            modelBuilder.Entity<CustomerDetail>()
                .Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(200);

            modelBuilder.Entity<CustomerDetail>()
                .Property(c => c.Email)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<CustomerDetail>()
                .Property(c => c.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            modelBuilder.Entity<InventoryItem>()
                .Property(item => item.UnitPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PurchaseRequest>()
                .HasIndex(pr => pr.PurchaseRequestCode)
                .IsUnique();

            modelBuilder.Entity<PurchaseRequest>()
                .Property(pr => pr.PurchaseRequestCode)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<PurchaseRequest>()
                .HasMany(pr => pr.PRItems)
                .WithOne(item => item.PurchaseRequest)
                .HasForeignKey(item => item.PurchaseRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PurchaseRequestItem>()
                .Property(item => item.Price)
                .HasPrecision(18, 2);

            // Order configurations - MOVED INSIDE OnModelCreating
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Table)
                .WithMany()
                .HasForeignKey(o => o.TableId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.MenuItem)
                .WithMany()
                .HasForeignKey(oi => oi.MenuItemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.UnitPrice)
                .HasPrecision(18, 2);
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