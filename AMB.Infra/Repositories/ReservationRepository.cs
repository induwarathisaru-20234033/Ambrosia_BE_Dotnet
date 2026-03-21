using AMB.Application.Interfaces.Repositories;
using AMB.Domain.Entities;
using AMB.Domain.Enums;
using AMB.Infra.DBContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMB.Infra.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly AMBContext _context;

        public ReservationRepository(AMBContext context)
        {
            _context = context;
        }

        public async Task<Reservation?> GetReservationByIdAsync(int id)
        {
            return await _context.Reservations
                .Include(r => r.CustomerDetail)
                .Include(r => r.BookingSlot)
                .Include(r => r.Table)
                .Include(r => r.AssignedWaiter)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id && r.Status == (int)EntityStatus.Active);
        }

        public async Task<Reservation?> GetReservationByCodeAsync(string reservationCode)
        {
            return await _context.Reservations
                .Include(r => r.CustomerDetail)
                .Include(r => r.BookingSlot)
                .Include(r => r.Table)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.ReservationCode == reservationCode && r.Status == (int)EntityStatus.Active);
        }

        public async Task<List<Reservation>> GetAllReservationsAsync()
        {
            return await _context.Reservations
                .Include(r => r.CustomerDetail)
                .Include(r => r.BookingSlot)
                .Include(r => r.Table)
                .Where(r => r.Status == (int)EntityStatus.Active)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Reservation> AddReservationAsync(Reservation reservation)
        {
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            return await _context.Reservations
                .Include(r => r.CustomerDetail)
                .Include(r => r.BookingSlot)
                .Include(r => r.Table)
                .Include(r => r.AssignedWaiter)
                .AsNoTracking()
                .FirstAsync(r => r.Id == reservation.Id);
        }

        public async Task UpdateReservationAsync(Reservation reservation)
        {
            _context.Reservations.Update(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteReservationAsync(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                reservation.Status = (int)EntityStatus.Inactive;
                reservation.DeletedDate = DateTimeOffset.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> HasBookingSlotReservationsOnDateAsync(int bookingSlotId, DateOnly date)
        {
            return await _context.Reservations
                .AnyAsync(r => r.BookingSlotId == bookingSlotId &&
                              DateOnly.FromDateTime(r.ReservationDate.Date) == date &&
                              r.Status == (int)EntityStatus.Active);
        }

        public async Task<List<Reservation>> GetBookingSlotReservationsByDateAsync(int bookingSlotId, DateOnly date)
        {
            return await _context.Reservations
                .Include(r => r.CustomerDetail)
                .Include(r => r.BookingSlot)
                .Include(r => r.Table)
                .Where(r => r.BookingSlotId == bookingSlotId &&
                           DateOnly.FromDateTime(r.ReservationDate.Date) == date &&
                           r.Status == (int)EntityStatus.Active)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> HasTableReservationsForTimeSlotAsync(int tableId, DateOnly date, TimeOnly startTime, TimeOnly endTime)
        {
            return await _context.Reservations
                .Include(r => r.BookingSlot)
                .AnyAsync(r => r.TableId == tableId &&
                              DateOnly.FromDateTime(r.ReservationDate.Date) == date &&
                              r.BookingSlot.StartTime >= startTime &&
                              r.BookingSlot.EndTime <= endTime &&
                              r.Status == (int)EntityStatus.Active);
        }

        public async Task<List<Reservation>> GetTableReservationsByDateAndTimeSlotAsync(int tableId, DateOnly date, TimeOnly startTime, TimeOnly endTime)
        {
            return await _context.Reservations
                .Include(r => r.CustomerDetail)
                .Include(r => r.BookingSlot)
                .Include(r => r.Table)
                .Where(r => r.TableId == tableId &&
                           DateOnly.FromDateTime(r.ReservationDate.Date) == date &&
                           r.BookingSlot.StartTime >= startTime &&
                           r.BookingSlot.EndTime <= endTime &&
                           r.Status == (int)EntityStatus.Active)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Reservation>> GetReservationsByDateAsync(DateOnly date)
        {
            return await _context.Reservations
                .Include(r => r.CustomerDetail)
                .Include(r => r.BookingSlot)
                .Include(r => r.Table)
                .Where(r => DateOnly.FromDateTime(r.ReservationDate.Date) == date &&
                           r.Status == (int)EntityStatus.Active)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Reservation>> GetTableReservationsByDateAsync(int tableId, DateOnly date)
        {
            return await _context.Reservations
                .Include(r => r.CustomerDetail)
                .Include(r => r.BookingSlot)
                .Include(r => r.Table)
                .Where(r => r.TableId == tableId &&
                           DateOnly.FromDateTime(r.ReservationDate.Date) == date &&
                           r.Status == (int)EntityStatus.Active)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Reservation?> MarkAsCancelledAsync(int reservationId, DateTimeOffset cancelledAt)
        {
            return await UpdateReservationStatusAsync(reservationId, ReservationStatus.Cancelled, cancelledAt);
        }

        public async Task<Reservation?> MarkAsArrivedAsync(int reservationId, DateTimeOffset arrivedAt)
        {
            return await UpdateReservationStatusAsync(reservationId, ReservationStatus.Arrived, arrivedAt);
        }

        public async Task<Reservation?> MarkAsNoShowAsync(int reservationId, DateTimeOffset noShowMarkedAt)
        {
            return await UpdateReservationStatusAsync(reservationId, ReservationStatus.NoShow, noShowMarkedAt);
        }

        public async Task<Reservation?> AssignWaiterAsync(int reservationId, int waiterId)
        {
            var reservation = await GetTrackedActiveReservationAsync(reservationId);
            if (reservation == null) return null;

            reservation.AssignedWaiterId = waiterId;

            // Deactivate any existing active assignment for this reservation
            var existingAssignment = await _context.ReservationWaiterAssignments
                .FirstOrDefaultAsync(a => a.ReservationId == reservationId && a.Status == (int)EntityStatus.Active);

            if (existingAssignment != null)
            {
                existingAssignment.Status = (int)EntityStatus.Inactive;
                existingAssignment.UnassignedAt = DateTimeOffset.UtcNow;
            }

            // Add the new junction record
            _context.ReservationWaiterAssignments.Add(new ReservationWaiterAssignment
            {
                ReservationId = reservationId,
                WaiterId = waiterId,
                AssignedAt = DateTimeOffset.UtcNow,
                Status = (int)EntityStatus.Active
            });

            await _context.SaveChangesAsync();

            return await _context.Reservations
                .Include(r => r.CustomerDetail)
                .Include(r => r.BookingSlot)
                .Include(r => r.Table)
                .Include(r => r.AssignedWaiter)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == reservationId);
        }

        public async Task<Reservation?> UnassignWaiterAsync(int reservationId)
        {
            var reservation = await GetTrackedActiveReservationAsync(reservationId);
            if (reservation == null) return null;

            reservation.AssignedWaiterId = null;

            // Deactivate the active junction record
            var activeAssignment = await _context.ReservationWaiterAssignments
                .FirstOrDefaultAsync(a => a.ReservationId == reservationId && a.Status == (int)EntityStatus.Active);

            if (activeAssignment != null)
            {
                activeAssignment.Status = (int)EntityStatus.Inactive;
                activeAssignment.UnassignedAt = DateTimeOffset.UtcNow;
            }

            await _context.SaveChangesAsync();

            return await _context.Reservations
                .Include(r => r.CustomerDetail)
                .Include(r => r.BookingSlot)
                .Include(r => r.Table)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == reservationId);
        }

        public async Task<List<Reservation>> GetUnassignedReservationsAsync(DateOnly date)
        {
            return await _context.Reservations
                .Include(r => r.CustomerDetail)
                .Include(r => r.BookingSlot)
                .Include(r => r.Table)
                .Where(r => r.AssignedWaiterId == null &&
                            DateOnly.FromDateTime(r.ReservationDate.Date) == date &&
                            r.Status == (int)EntityStatus.Active)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<ReservationWaiterAssignment>> GetActiveWaiterAssignmentsAsync(DateOnly date)
        {
            return await _context.ReservationWaiterAssignments
                .Include(a => a.Reservation)
                .Include(a => a.Waiter)
                .Where(a => a.Status == (int)EntityStatus.Active &&
                            DateOnly.FromDateTime(a.Reservation.ReservationDate.Date) == date)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Reservation>> GetCurrentAssignedReservationsAsync(DateTimeOffset fromDate)
        {
            return await _context.Reservations
                .Include(r => r.CustomerDetail)
                .Include(r => r.BookingSlot)
                .Include(r => r.Table)
                .Include(r => r.AssignedWaiter)
                .Where(r => r.Status == (int)EntityStatus.Active &&
                            r.AssignedWaiterId != null &&
                            r.ReservationDate >= fromDate &&
                            (r.ReservationStatus == (int)ReservationStatus.Booked ||
                             r.ReservationStatus == (int)ReservationStatus.Arrived))
                .AsNoTracking()
                .ToListAsync();
        }

        private async Task<Reservation?> UpdateReservationStatusAsync(
            int reservationId,
            ReservationStatus targetStatus,
            DateTimeOffset actionAt)
        {
            var reservation = await GetTrackedActiveReservationAsync(reservationId);
            if (reservation == null)
            {
                return null;
            }

            reservation.ReservationStatus = (int)targetStatus;

            switch (targetStatus)
            {
                case ReservationStatus.Cancelled:
                    reservation.CancelledAt = actionAt;
                    reservation.ArrivedAt = null;
                    reservation.NoShowMarkedAt = null;
                    break;
                case ReservationStatus.Arrived:
                    reservation.ArrivedAt = actionAt;
                    reservation.NoShowMarkedAt = null;
                    break;
                case ReservationStatus.NoShow:
                    reservation.NoShowMarkedAt = actionAt;
                    reservation.ArrivedAt = null;
                    break;
            }

            await _context.SaveChangesAsync();
            return reservation;
        }

        private async Task<Reservation?> GetTrackedActiveReservationAsync(int reservationId)
        {
            var reservation = await _context.Reservations
                .Include(r => r.CustomerDetail)
                .Include(r => r.BookingSlot)
                .Include(r => r.Table)
                .FirstOrDefaultAsync(r => r.Id == reservationId && r.Status == (int)EntityStatus.Active);

            return reservation;
        }

        public IQueryable<Reservation> GetSearchQuery()
        {
            return _context.Reservations
                .Include(r => r.CustomerDetail)
                .Include(r => r.BookingSlot)
                .Include(r => r.Table)
                .Include(r => r.AssignedWaiter)
                .Where(r => r.Status == (int)EntityStatus.Active)
                .AsNoTracking();
        }
    }
}
