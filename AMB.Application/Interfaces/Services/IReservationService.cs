using AMB.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMB.Application.Interfaces.Services
{
    public interface IReservationService
    {
        Task<ReservationDto> CreateReservationAsync(CreateReservationRequestDto request);
        Task<ReservationDto?> GetReservationByIdAsync(int id);
        Task<ReservationDto?> GetReservationByCodeAsync(string reservationCode);
        Task<List<ReservationDto>> GetAllReservationsAsync();
        Task<List<ReservationDto>> GetReservationsByDateAsync(DateOnly date);
        Task<PagedResponseDto<ReservationDto>> GetReservationsPagedAsync(ReservationFilterRequestDto filter);
        Task<ReservationAvailabilityDto> CheckAvailabilityAsync(int bookingSlotId, int tableId, DateOnly date);
        Task<List<ReservationDto>> GetBookingSlotReservationsAsync(int bookingSlotId, DateOnly date);
        Task<List<ReservationDto>> GetTableReservationsAsync(int tableId, DateOnly date);
        Task<ReservationDto?> CancelReservationAsync(int reservationId);
        Task<ReservationDto?> MarkReservationAsArrivedAsync(int reservationId);
        Task<ReservationDto?> MarkReservationAsNoShowAsync(int reservationId);
    }
}
