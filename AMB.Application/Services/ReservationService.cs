using AMB.Application.Dtos;
using AMB.Application.Interfaces.Repositories;
using AMB.Application.Interfaces.Services;
using AMB.Domain.Entities;
using AMB.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMB.Application.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IConfigRepository _configRepository;

        public ReservationService(
            IReservationRepository reservationRepository,
            IConfigRepository configRepository)
        {
            _reservationRepository = reservationRepository;
            _configRepository = configRepository;
        }

        public async Task<ReservationDto> CreateReservationAsync(CreateReservationRequestDto request)
        {
            // Check availability before creating
            var availability = await CheckAvailabilityAsync(
                request.BookingSlotId,
                request.TableId,
                DateOnly.FromDateTime(request.ReservationDate.Date));

            if (!availability.IsSlotAvailable || !availability.IsTableAvailable)
            {
                throw new InvalidOperationException(
                    $"The selected slot or table is not available. " +
                    $"Slot has {availability.ExistingReservationsForSlot} existing reservations. " +
                    $"Table has {availability.ExistingReservationsForTable} existing reservations for this time slot.");
            }

            // Create customer detail
            var customerDetail = new CustomerDetail
            {
                Name = request.CustomerName,
                Email = request.CustomerEmail,
                PhoneNumber = request.CustomerPhoneNumber,
                Status = (int)EntityStatus.Active,
                CreatedDate = DateTimeOffset.UtcNow
            };

            // Create reservation
            var reservation = new Reservation
            {
                ReservationCode = GenerateReservationCode(),
                PartySize = request.PartySize,
                ReservationStatus = (int)ReservationStatus.Booked,
                ReservationDate = request.ReservationDate,
                Occasion = request.Occasion,
                SpecialRequests = request.SpecialRequests,
                CustomerDetailId = customerDetail.Id,
                CustomerDetail = customerDetail,
                BookingSlotId = request.BookingSlotId,
                TableId = request.TableId,
                Status = (int)EntityStatus.Active,
                CreatedDate = DateTimeOffset.UtcNow
            };

            var createdReservation = await _reservationRepository.AddReservationAsync(reservation);
            return MapToDto(createdReservation);
        }

        public async Task<ReservationDto?> GetReservationByIdAsync(int id)
        {
            var reservation = await _reservationRepository.GetReservationByIdAsync(id);
            return reservation == null ? null : MapToDto(reservation);
        }

        public async Task<ReservationDto?> GetReservationByCodeAsync(string reservationCode)
        {
            var reservation = await _reservationRepository.GetReservationByCodeAsync(reservationCode);
            return reservation == null ? null : MapToDto(reservation);
        }

        public async Task<List<ReservationDto>> GetAllReservationsAsync()
        {
            var reservations = await _reservationRepository.GetAllReservationsAsync();
            return reservations.Select(MapToDto).ToList();
        }

        public async Task<List<ReservationDto>> GetReservationsByDateAsync(DateOnly date)
        {
            var reservations = await _reservationRepository.GetReservationsByDateAsync(date);
            return reservations.Select(MapToDto).ToList();
        }

        /// <summary>
        /// Check if a booking slot and table are available on a specific date
        /// This demonstrates the tracking functionality you requested
        /// </summary>
        public async Task<ReservationAvailabilityDto> CheckAvailabilityAsync(int bookingSlotId, int tableId, DateOnly date)
        {
            // Get the booking slot to retrieve time information
            var bookingSlots = await _configRepository.GetAllBookingSlotsAsync();
            var bookingSlot = bookingSlots.FirstOrDefault(bs => bs.Id == bookingSlotId);

            if (bookingSlot == null)
            {
                throw new ArgumentException($"BookingSlot with ID {bookingSlotId} not found.");
            }

            // Check if the BookingSlot has other reservations on that day
            var hasSlotReservations = await _reservationRepository.HasBookingSlotReservationsOnDateAsync(bookingSlotId, date);
            var slotReservations = await _reservationRepository.GetBookingSlotReservationsByDateAsync(bookingSlotId, date);

            // Check if the Table has reservations on that day for that time slot
            var hasTableReservations = await _reservationRepository.HasTableReservationsForTimeSlotAsync(
                tableId, date, bookingSlot.StartTime, bookingSlot.EndTime);
            var tableReservations = await _reservationRepository.GetTableReservationsByDateAndTimeSlotAsync(
                tableId, date, bookingSlot.StartTime, bookingSlot.EndTime);

            return new ReservationAvailabilityDto
            {
                BookingSlotId = bookingSlotId,
                TableId = tableId,
                Date = date,
                StartTime = bookingSlot.StartTime,
                EndTime = bookingSlot.EndTime,
                IsSlotAvailable = !hasSlotReservations,
                IsTableAvailable = !hasTableReservations,
                ExistingReservationsForSlot = slotReservations.Count,
                ExistingReservationsForTable = tableReservations.Count
            };
        }

        public async Task<List<ReservationDto>> GetBookingSlotReservationsAsync(int bookingSlotId, DateOnly date)
        {
            var reservations = await _reservationRepository.GetBookingSlotReservationsByDateAsync(bookingSlotId, date);
            return reservations.Select(MapToDto).ToList();
        }

        public async Task<List<ReservationDto>> GetTableReservationsAsync(int tableId, DateOnly date)
        {
            var reservations = await _reservationRepository.GetTableReservationsByDateAsync(tableId, date);
            return reservations.Select(MapToDto).ToList();
        }

        public async Task<ReservationDto?> CancelReservationAsync(int reservationId)
        {
            return await ChangeReservationStatusAsync(
                reservationId,
                existing =>
                {
                    if (existing.ReservationStatus == (int)ReservationStatus.Arrived)
                    {
                        throw new InvalidOperationException("Arrived reservations cannot be cancelled.");
                    }
                },
                _reservationRepository.MarkAsCancelledAsync);
        }

        public async Task<ReservationDto?> MarkReservationAsArrivedAsync(int reservationId)
        {
            return await ChangeReservationStatusAsync(
                reservationId,
                existing =>
                {
                    if (existing.ReservationStatus == (int)ReservationStatus.Cancelled)
                    {
                        throw new InvalidOperationException("Cancelled reservations cannot be marked as arrived.");
                    }
                },
                _reservationRepository.MarkAsArrivedAsync);
        }

        public async Task<ReservationDto?> MarkReservationAsNoShowAsync(int reservationId)
        {
            return await ChangeReservationStatusAsync(
                reservationId,
                existing =>
                {
                    if (existing.ReservationStatus == (int)ReservationStatus.Arrived)
                    {
                        throw new InvalidOperationException("Arrived reservations cannot be marked as no-show.");
                    }

                    if (existing.ReservationStatus == (int)ReservationStatus.Cancelled)
                    {
                        throw new InvalidOperationException("Cancelled reservations cannot be marked as no-show.");
                    }
                },
                _reservationRepository.MarkAsNoShowAsync);
        }

        private async Task<ReservationDto?> ChangeReservationStatusAsync(
            int reservationId,
            Action<Reservation> validateTransition,
            Func<int, DateTimeOffset, Task<Reservation?>> applyStatusChange)
        {
            var existing = await _reservationRepository.GetReservationByIdAsync(reservationId);
            if (existing == null)
            {
                return null;
            }

            validateTransition(existing);

            var updated = await applyStatusChange(reservationId, DateTimeOffset.UtcNow);
            return updated == null ? null : MapToDto(updated);
        }

        private static ReservationDto MapToDto(Reservation reservation)
        {
            return new ReservationDto
            {
                Id = reservation.Id,
                ReservationCode = reservation.ReservationCode,
                PartySize = reservation.PartySize,
                ReservationStatus = reservation.ReservationStatus,
                ReservationDate = reservation.ReservationDate,
                Occasion = reservation.Occasion,
                SpecialRequests = reservation.SpecialRequests,
                ArrivedAt = reservation.ArrivedAt,
                NoShowMarkedAt = reservation.NoShowMarkedAt,
                CancelledAt = reservation.CancelledAt,
                CustomerDetail = reservation.CustomerDetail == null ? null : new CustomerDetailDto
                {
                    Id = reservation.CustomerDetail.Id,
                    Name = reservation.CustomerDetail.Name,
                    Email = reservation.CustomerDetail.Email,
                    PhoneNumber = reservation.CustomerDetail.PhoneNumber
                },
                BookingSlot = reservation.BookingSlot == null ? null : new BookingSlotDto
                {
                    Id = reservation.BookingSlot.Id,
                    SlotId = reservation.BookingSlot.SlotId,
                    StartTime = reservation.BookingSlot.StartTime,
                    EndTime = reservation.BookingSlot.EndTime,
                    Day = reservation.BookingSlot.Day
                },
                Table = reservation.Table == null ? null : new TableDto
                {
                    Id = reservation.Table.Id,
                    TableName = reservation.Table.TableName,
                    Capacity = reservation.Table.Capacity,
                    IsOnlineBookingEnabled = reservation.Table.IsOnlineBookingEnabled
                }
            };
        }

        private static string GenerateReservationCode()
        {
            // Generate a unique reservation code (e.g., RES-20260308-ABCD)
            var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
            var randomPart = Guid.NewGuid().ToString("N").Substring(0, 4).ToUpper();
            return $"RES-{datePart}-{randomPart}";
        }
    }
}
