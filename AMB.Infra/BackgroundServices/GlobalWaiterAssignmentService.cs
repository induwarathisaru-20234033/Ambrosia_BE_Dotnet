using AMB.Domain.Constants;
using AMB.Domain.Entities;
using AMB.Domain.Enums;
using AMB.Infra.DBContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace AMB.Infra.BackgroundServices
{
    public class GlobalWaiterAssignmentService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<GlobalWaiterAssignmentService> _logger;

        public GlobalWaiterAssignmentService(IServiceProvider serviceProvider, ILogger<GlobalWaiterAssignmentService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("GlobalWaiterAssignmentService started at {StartedAtUtc}", DateTimeOffset.UtcNow);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<AMBContext>();
                        await AssignWaitersAsync(context, stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled error in GlobalWaiterAssignmentService loop.");
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }

            _logger.LogInformation("GlobalWaiterAssignmentService stopped at {StoppedAtUtc}", DateTimeOffset.UtcNow);
        }

        private async Task AssignWaitersAsync(AMBContext context, CancellationToken stoppingToken)
        {
            const int maxTablesPerWaiter = 5;

            var today = DateTimeOffset.UtcNow.Date;

            _logger.LogInformation(
                "GlobalWaiterAssignmentService cycle started at {CycleAtUtc}. Looking for arrived reservations without assigned waiter from {Today} onward.",
                DateTimeOffset.UtcNow,
                today);

            var pendingReservations = await context.Reservations
                .Include(r => r.Table)
                .Where(r => r.ReservationStatus == (int)ReservationStatus.Arrived && r.AssignedWaiterId == null && r.ReservationDate >= today)
                .OrderBy(r => r.ArrivedAt)
                .ToListAsync(stoppingToken);

            _logger.LogInformation(
                "Found {PendingCount} pending reservations for waiter auto-assignment.",
                pendingReservations.Count);

            if (!pendingReservations.Any())
            {
                _logger.LogDebug("No pending reservations found in this cycle.");
                return;
            }

            foreach (var reservation in pendingReservations)
            {
                _logger.LogInformation(
                    "Evaluating reservation {ReservationId} ({ReservationCode}) with party size {PartySize}.",
                    reservation.Id,
                    reservation.ReservationCode,
                    reservation.PartySize);

                var bestWaiter = await context.Employees
                    .Where(e => e.IsOnline && e.EmployeeRoleMaps
                        .Any(map => map.Role.RoleName == Domain.Constants.Role.WaiterRole))
                    .Select(e => new
                    {
                        Waiter = e,
                        // Count active "Arrived" reservations for this waiter TODAY
                        ActiveTableCount = context.Reservations
                            .Count(r => r.AssignedWaiterId == e.Id
                                    && r.ReservationStatus == (int)ReservationStatus.Arrived
                                    && r.ReservationDate >= today),

                        // Total guests currently being served by this waiter
                        CurrentGuestLoad = context.Reservations
                            .Where(r => r.AssignedWaiterId == e.Id
                                    && r.ReservationStatus == (int)ReservationStatus.Arrived
                                    && r.ReservationDate >= today)
                            .Sum(r => (int?)r.PartySize) ?? 0
                    })
                    // Constraint 2: Only include waiters under the Table Capacity limit
                    .Where(x => x.ActiveTableCount < maxTablesPerWaiter)
                    // Order by Guest Load first (fewer people), then by Table Count
                    .OrderBy(x => x.CurrentGuestLoad)
                    .ThenBy(x => x.ActiveTableCount)
                    .Select(x => x.Waiter)
                    .FirstOrDefaultAsync(stoppingToken);

                if (bestWaiter != null)
                {
                    reservation.AssignedWaiterId = bestWaiter.Id;

                    var history = new ReservationWaiterAssignment
                    {
                        ReservationId = reservation.Id,
                        WaiterId = bestWaiter.Id,
                        AssignedAt = DateTimeOffset.UtcNow,
                        Status = 1
                    };

                    context.ReservationWaiterAssignments.Add(history);

                    _logger.LogInformation(
                        "Assigned waiter {WaiterId} ({WaiterName}) to reservation {ReservationId} ({ReservationCode}), party size {PartySize}.",
                        bestWaiter.Id,
                        bestWaiter.FirstName,
                        reservation.Id,
                        reservation.ReservationCode,
                        reservation.PartySize);

                    // Save immediately to update counts for the next reservation in the loop
                    await context.SaveChangesAsync(stoppingToken);
                }
                else
                {
                    _logger.LogWarning(
                        "No available waiter found for reservation {ReservationId} ({ReservationCode}) with party size {PartySize}.",
                        reservation.Id,
                        reservation.ReservationCode,
                        reservation.PartySize);
                }
            }

            _logger.LogInformation("GlobalWaiterAssignmentService cycle completed at {CompletedAtUtc}", DateTimeOffset.UtcNow);
        }
    }
}
