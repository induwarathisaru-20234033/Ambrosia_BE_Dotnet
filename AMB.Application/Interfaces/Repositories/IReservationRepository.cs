using AMB.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMB.Application.Interfaces.Repositories
{
    public interface IReservationRepository
    {
        Task<Reservation?> GetReservationByIdAsync(int id);
        Task<Reservation?> GetReservationByCodeAsync(string reservationCode);
        Task<List<Reservation>> GetAllReservationsAsync();
        Task<Reservation> AddReservationAsync(Reservation reservation);
        Task UpdateReservationAsync(Reservation reservation);
        Task DeleteReservationAsync(int id);
        Task<bool> HasBookingSlotReservationsOnDateAsync(int bookingSlotId, DateOnly date);
        Task<List<Reservation>> GetBookingSlotReservationsByDateAsync(int bookingSlotId, DateOnly date);
        Task<bool> HasTableReservationsForTimeSlotAsync(int tableId, DateOnly date, TimeOnly startTime, TimeOnly endTime);
        Task<List<Reservation>> GetTableReservationsByDateAndTimeSlotAsync(int tableId, DateOnly date, TimeOnly startTime, TimeOnly endTime);
        Task<List<Reservation>> GetReservationsByDateAsync(DateOnly date);
        Task<List<Reservation>> GetTableReservationsByDateAsync(int tableId, DateOnly date);
        Task<Reservation?> MarkAsCancelledAsync(int reservationId, DateTimeOffset cancelledAt);
        Task<Reservation?> MarkAsArrivedAsync(int reservationId, DateTimeOffset arrivedAt);
        Task<Reservation?> MarkAsNoShowAsync(int reservationId, DateTimeOffset noShowMarkedAt);
        Task<Reservation?> AssignWaiterAsync(int reservationId, int waiterId);
        Task<Reservation?> UnassignWaiterAsync(int reservationId);
        Task<List<Reservation>> GetUnassignedReservationsAsync(DateOnly date);
        Task<List<ReservationWaiterAssignment>> GetActiveWaiterAssignmentsAsync(DateOnly date);
        Task<List<Reservation>> GetCurrentAssignedReservationsAsync(DateTimeOffset fromDate);
        IQueryable<Reservation> GetSearchQuery();
        Task<Reservation?> GetActiveReservationByTableGuidAsync(Guid tableGuid);
    }
}
