using AMB.Application.Interfaces.Repositories;
using AMB.Domain.Entities;
using AMB.Domain.Enums;

namespace AMB.Tests.Mocks
{
    internal sealed class TestReservationRepository : IReservationRepository
    {
        public Reservation? LastAddedReservation { get; private set; }
        public Reservation? LastUpdatedReservation { get; private set; }
        public Dictionary<int, Reservation> Reservations { get; } = new();
        private readonly List<ReservationWaiterAssignment> _waiterAssignments = new();
        private int _nextId = 1;

        public Task<Reservation> AddReservationAsync(Reservation reservation)
        {
            reservation.Id = _nextId++;
            if (reservation.CustomerDetail != null && reservation.CustomerDetail.Id == 0)
            {
                reservation.CustomerDetail.Id = _nextId++;
                reservation.CustomerDetailId = reservation.CustomerDetail.Id;
            }
            LastAddedReservation = reservation;
            Reservations[reservation.Id] = reservation;
            return Task.FromResult(reservation);
        }

        public Task<Reservation?> GetReservationByIdAsync(int id)
        {
            Reservations.TryGetValue(id, out var reservation);
            return Task.FromResult(reservation);
        }

        public Task<Reservation?> GetReservationByCodeAsync(string reservationCode)
        {
            var reservation = Reservations.Values
                .FirstOrDefault(r => r.ReservationCode == reservationCode && r.Status == (int)EntityStatus.Active);
            return Task.FromResult(reservation);
        }

        public Task<List<Reservation>> GetAllReservationsAsync()
        {
            var reservations = Reservations.Values
                .Where(r => r.Status == (int)EntityStatus.Active)
                .ToList();
            return Task.FromResult(reservations);
        }

        public Task UpdateReservationAsync(Reservation reservation)
        {
            Reservations[reservation.Id] = reservation;
            LastUpdatedReservation = reservation;
            return Task.CompletedTask;
        }

        public Task DeleteReservationAsync(int id)
        {
            if (Reservations.TryGetValue(id, out var reservation))
            {
                reservation.Status = (int)EntityStatus.Inactive;
                reservation.DeletedDate = DateTimeOffset.UtcNow;
            }
            return Task.CompletedTask;
        }

        public Task<bool> HasBookingSlotReservationsOnDateAsync(int bookingSlotId, DateOnly date)
        {
            var hasReservations = Reservations.Values.Any(r =>
                r.BookingSlotId == bookingSlotId &&
                DateOnly.FromDateTime(r.ReservationDate.Date) == date &&
                r.Status == (int)EntityStatus.Active);
            return Task.FromResult(hasReservations);
        }

        public Task<List<Reservation>> GetBookingSlotReservationsByDateAsync(int bookingSlotId, DateOnly date)
        {
            var reservations = Reservations.Values
                .Where(r => r.BookingSlotId == bookingSlotId &&
                           DateOnly.FromDateTime(r.ReservationDate.Date) == date &&
                           r.Status == (int)EntityStatus.Active)
                .ToList();
            return Task.FromResult(reservations);
        }

        public Task<bool> HasTableReservationsForTimeSlotAsync(int tableId, DateOnly date, TimeOnly startTime, TimeOnly endTime)
        {
            var hasReservations = Reservations.Values.Any(r =>
                r.TableId == tableId &&
                DateOnly.FromDateTime(r.ReservationDate.Date) == date &&
                r.BookingSlot != null &&
                r.BookingSlot.StartTime >= startTime &&
                r.BookingSlot.EndTime <= endTime &&
                r.Status == (int)EntityStatus.Active);
            return Task.FromResult(hasReservations);
        }

        public Task<List<Reservation>> GetTableReservationsByDateAndTimeSlotAsync(int tableId, DateOnly date, TimeOnly startTime, TimeOnly endTime)
        {
            var reservations = Reservations.Values
                .Where(r => r.TableId == tableId &&
                           DateOnly.FromDateTime(r.ReservationDate.Date) == date &&
                           r.BookingSlot != null &&
                           r.BookingSlot.StartTime >= startTime &&
                           r.BookingSlot.EndTime <= endTime &&
                           r.Status == (int)EntityStatus.Active)
                .ToList();
            return Task.FromResult(reservations);
        }

        public Task<List<Reservation>> GetReservationsByDateAsync(DateOnly date)
        {
            var reservations = Reservations.Values
                .Where(r => DateOnly.FromDateTime(r.ReservationDate.Date) == date &&
                           r.Status == (int)EntityStatus.Active)
                .ToList();
            return Task.FromResult(reservations);
        }

        public Task<List<Reservation>> GetTableReservationsByDateAsync(int tableId, DateOnly date)
        {
            var reservations = Reservations.Values
                .Where(r => r.TableId == tableId &&
                           DateOnly.FromDateTime(r.ReservationDate.Date) == date &&
                           r.Status == (int)EntityStatus.Active)
                .ToList();
            return Task.FromResult(reservations);
        }

        public Task<Reservation?> MarkAsCancelledAsync(int reservationId, DateTimeOffset cancelledAt)
        {
            if (!Reservations.TryGetValue(reservationId, out var reservation))
            {
                return Task.FromResult<Reservation?>(null);
            }

            if (reservation.Status != (int)EntityStatus.Active)
            {
                return Task.FromResult<Reservation?>(null);
            }

            reservation.ReservationStatus = (int)ReservationStatus.Cancelled;
            reservation.CancelledAt = cancelledAt;
            reservation.ArrivedAt = null;
            reservation.NoShowMarkedAt = null;
            LastUpdatedReservation = reservation;
            return Task.FromResult<Reservation?>(reservation);
        }

        public Task<Reservation?> MarkAsArrivedAsync(int reservationId, DateTimeOffset arrivedAt)
        {
            if (!Reservations.TryGetValue(reservationId, out var reservation))
            {
                return Task.FromResult<Reservation?>(null);
            }

            if (reservation.Status != (int)EntityStatus.Active)
            {
                return Task.FromResult<Reservation?>(null);
            }

            reservation.ReservationStatus = (int)ReservationStatus.Arrived;
            reservation.ArrivedAt = arrivedAt;
            reservation.NoShowMarkedAt = null;
            LastUpdatedReservation = reservation;
            return Task.FromResult<Reservation?>(reservation);
        }

        public Task<Reservation?> MarkAsNoShowAsync(int reservationId, DateTimeOffset noShowMarkedAt)
        {
            if (!Reservations.TryGetValue(reservationId, out var reservation))
            {
                return Task.FromResult<Reservation?>(null);
            }

            if (reservation.Status != (int)EntityStatus.Active)
            {
                return Task.FromResult<Reservation?>(null);
            }

            reservation.ReservationStatus = (int)ReservationStatus.NoShow;
            reservation.NoShowMarkedAt = noShowMarkedAt;
            reservation.ArrivedAt = null;
            LastUpdatedReservation = reservation;
            return Task.FromResult<Reservation?>(reservation);
        }

        public Task<Reservation?> AssignWaiterAsync(int reservationId, int waiterId)
        {
            if (!Reservations.TryGetValue(reservationId, out var reservation))
                return Task.FromResult<Reservation?>(null);

            // Deactivate existing active assignment
            var existing = _waiterAssignments
                .FirstOrDefault(a => a.ReservationId == reservationId && a.Status == (int)EntityStatus.Active);
            if (existing != null)
            {
                existing.Status = (int)EntityStatus.Inactive;
                existing.UnassignedAt = DateTimeOffset.UtcNow;
            }

            _waiterAssignments.Add(new ReservationWaiterAssignment
            {
                Id = _waiterAssignments.Count + 1,
                ReservationId = reservationId,
                WaiterId = waiterId,
                AssignedAt = DateTimeOffset.UtcNow,
                Status = (int)EntityStatus.Active
            });

            reservation.AssignedWaiterId = waiterId;
            LastUpdatedReservation = reservation;
            return Task.FromResult<Reservation?>(reservation);
        }

        public Task<Reservation?> UnassignWaiterAsync(int reservationId)
        {
            if (!Reservations.TryGetValue(reservationId, out var reservation))
                return Task.FromResult<Reservation?>(null);

            var active = _waiterAssignments
                .FirstOrDefault(a => a.ReservationId == reservationId && a.Status == (int)EntityStatus.Active);
            if (active != null)
            {
                active.Status = (int)EntityStatus.Inactive;
                active.UnassignedAt = DateTimeOffset.UtcNow;
            }

            reservation.AssignedWaiterId = null;
            reservation.AssignedWaiter = null;
            LastUpdatedReservation = reservation;
            return Task.FromResult<Reservation?>(reservation);
        }

        public Task<List<Reservation>> GetUnassignedReservationsAsync(DateOnly date)
        {
            var result = Reservations.Values
                .Where(r => r.AssignedWaiterId == null &&
                            DateOnly.FromDateTime(r.ReservationDate.Date) == date &&
                            r.Status == (int)EntityStatus.Active)
                .ToList();
            return Task.FromResult(result);
        }

        public Task<List<ReservationWaiterAssignment>> GetActiveWaiterAssignmentsAsync(DateOnly date)
        {
            var result = _waiterAssignments
                .Where(a => a.Status == (int)EntityStatus.Active &&
                            Reservations.TryGetValue(a.ReservationId, out var r) &&
                            DateOnly.FromDateTime(r.ReservationDate.Date) == date)
                .ToList();
            return Task.FromResult(result);
        }

        public Task<List<Reservation>> GetCurrentAssignedReservationsAsync(DateTimeOffset fromDate)
        {
            var result = Reservations.Values
                .Where(r => r.Status == (int)EntityStatus.Active &&
                            r.AssignedWaiterId != null &&
                            r.ReservationDate >= fromDate &&
                            (r.ReservationStatus == (int)ReservationStatus.Booked ||
                             r.ReservationStatus == (int)ReservationStatus.Arrived))
                .OrderBy(r => r.ReservationDate)
                .ToList();

            return Task.FromResult(result);
        }

        public IQueryable<Reservation> GetSearchQuery()
        {
            return Reservations.Values
                .Where(r => r.Status == (int)EntityStatus.Active)
                .AsQueryable();
        }

        public Task<Reservation?> GetActiveReservationByTableGuidAsync(Guid tableGuid)
        {
            var reservation = Reservations.Values
                .FirstOrDefault(r => r.Table != null 
                                     && r.Table.QrIdentifier == tableGuid 
                                     && r.ReservationStatus == (int)ReservationStatus.Arrived
                                     && r.Status == (int)EntityStatus.Active);
            return Task.FromResult(reservation);
        }
    }
}
