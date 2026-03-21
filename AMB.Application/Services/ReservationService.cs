using AMB.Application.Dtos;
using AMB.Application.Interfaces.Repositories;
using AMB.Application.Interfaces.Services;
using AMB.Application.Mappers;
using AMB.Domain.Entities;
using AMB.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
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
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmailService _emailService;

        public ReservationService(
            IReservationRepository reservationRepository,
            IConfigRepository configRepository,
            IEmployeeRepository employeeRepository,
            IEmailService emailService)
        {
            _reservationRepository = reservationRepository;
            _configRepository = configRepository;
            _employeeRepository = employeeRepository;
            _emailService = emailService;
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

            if (createdReservation != null) 
            {
                var emailData = new
                {
                    CustomerName = createdReservation.CustomerDetail?.Name,
                    createdReservation.ReservationCode,
                    CustomerEmail = createdReservation.CustomerDetail?.Email
                };

                await _emailService.SendEmailAsync(
                    to: createdReservation.CustomerDetail?.Email,
                    subject: $"Your Reservation Confirmation: {createdReservation.ReservationCode}",
                    templateName: "reservation-created",
                    model: emailData
                );
            }

            return createdReservation.ToReservationDto();
        }

        public async Task<ReservationDto?> GetReservationByIdAsync(int id)
        {
            var reservation = await _reservationRepository.GetReservationByIdAsync(id);
            return reservation?.ToReservationDto();
        }

        public async Task<ReservationDto?> GetReservationByCodeAsync(string reservationCode)
        {
            var reservation = await _reservationRepository.GetReservationByCodeAsync(reservationCode);
            return reservation?.ToReservationDto();
        }

        public async Task<List<ReservationDto>> GetAllReservationsAsync()
        {
            var reservations = await _reservationRepository.GetAllReservationsAsync();
            return reservations.Select(r => r.ToReservationDto()).ToList();
        }

        public async Task<List<ReservationDto>> GetReservationsByDateAsync(DateOnly date)
        {
            var reservations = await _reservationRepository.GetReservationsByDateAsync(date);
            return reservations.Select(r => r.ToReservationDto()).ToList();
        }

        public async Task<PagedResponseDto<ReservationDto>> GetReservationsPagedAsync(ReservationFilterRequestDto filter)
        {
            var query = _reservationRepository.GetSearchQuery();

            // Apply filters
            if (!string.IsNullOrEmpty(filter.ReservationCode))
                query = query.Where(r => r.ReservationCode.Contains(filter.ReservationCode));

            if (filter.ReservationStatus.HasValue)
                query = query.Where(r => r.ReservationStatus == filter.ReservationStatus.Value);

            if (!string.IsNullOrEmpty(filter.CustomerName))
                query = query.Where(r => r.CustomerDetail.Name.Contains(filter.CustomerName));

            if (!string.IsNullOrEmpty(filter.CustomerEmail))
                query = query.Where(r => r.CustomerDetail.Email.Contains(filter.CustomerEmail));

            if (!string.IsNullOrEmpty(filter.CustomerPhone))
                query = query.Where(r => r.CustomerDetail.PhoneNumber.Contains(filter.CustomerPhone));

            if (!string.IsNullOrEmpty(filter.Table))
                query = query.Where(r => r.Table.TableName.Contains(filter.Table));

            if (filter.ReservationDateFrom.HasValue)
                query = query.Where(r => r.ReservationDate >= filter.ReservationDateFrom);

            if (filter.ReservationDateTo.HasValue)
                query = query.Where(r => r.ReservationDate <= filter.ReservationDateTo);

            if (filter.CreatedDateFrom.HasValue)
                query = query.Where(r => r.CreatedDate >= filter.CreatedDateFrom);

            if (filter.CreatedDateTo.HasValue)
                query = query.Where(r => r.CreatedDate <= filter.CreatedDateTo);

            var supportsAsyncQuery = query.Provider is IAsyncQueryProvider;

            // Get total count for pagination
            var totalCount = supportsAsyncQuery
                ? await query.CountAsync()
                : query.Count();

            var orderedQuery = query.OrderByDescending(r => r.CreatedDate);

            List<Reservation> reservationEntities;
            if (filter.PageSize == 0)
            {
                reservationEntities = supportsAsyncQuery
                    ? await orderedQuery.ToListAsync()
                    : orderedQuery.ToList();
            }
            else
            {
                var pagedQuery = orderedQuery
                    .Skip((filter.PageNumber - 1) * filter.PageSize)
                    .Take(filter.PageSize);

                reservationEntities = supportsAsyncQuery
                    ? await pagedQuery.ToListAsync()
                    : pagedQuery.ToList();
            }

            // Map to DTO
            var reservations = reservationEntities
                .Select(r => r.ToReservationDto())
                .ToList();

            // Return paged response
            return new PagedResponseDto<ReservationDto>
            {
                Items = reservations,
                TotalItemCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                PageCount = filter.PageSize == 0
                    ? (totalCount > 0 ? 1 : 0)
                    : (int)Math.Ceiling((double)totalCount / filter.PageSize)
            };
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
            return reservations.Select(r => r.ToReservationDto()).ToList();
        }

        public async Task<List<ReservationDto>> GetTableReservationsAsync(int tableId, DateOnly date)
        {
            var reservations = await _reservationRepository.GetTableReservationsByDateAsync(tableId, date);
            return reservations.Select(r => r.ToReservationDto()).ToList();
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
            return updated?.ToReservationDto();
        }

        private static string GenerateReservationCode()
        {
            // Generate a unique reservation code (e.g., RES-20260308-ABCD)
            var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
            var randomPart = Guid.NewGuid().ToString("N").Substring(0, 4).ToUpper();
            return $"RES-{datePart}-{randomPart}";
        }

        public async Task<List<ReservationDto>> AssignWaiterAsync(AssignWaiterRequestDto request)
        {
            if (request.EmployeeId <= 0)
            {
                throw new ArgumentException("Valid EmployeeId is required.");
            }

            var reservationIds = request.ReservationIds?
                .Where(id => id > 0)
                .Distinct()
                .ToList() ?? new List<int>();

            if (!reservationIds.Any())
            {
                throw new ArgumentException("At least one valid reservation id is required.");
            }

            var waiter = await _employeeRepository.GetByIdAsync(request.EmployeeId);
            if (waiter == null)
            {
                throw new KeyNotFoundException($"Employee with ID {request.EmployeeId} not found.");
            }

            var updatedReservations = new List<ReservationDto>();

            foreach (var reservationId in reservationIds)
            {
                var reservation = await _reservationRepository.GetReservationByIdAsync(reservationId);
                if (reservation == null)
                {
                    throw new KeyNotFoundException($"Reservation with ID {reservationId} not found.");
                }

                var updated = await _reservationRepository.AssignWaiterAsync(reservationId, request.EmployeeId);
                if (updated != null)
                {
                    updatedReservations.Add(updated.ToReservationDto());
                }
            }

            return updatedReservations;
        }

        public async Task<List<ReservationDto>> UnassignWaiterAsync(AssignWaiterRequestDto request)
        {
            if (request.EmployeeId <= 0)
            {
                throw new ArgumentException("Valid EmployeeId is required.");
            }

            var reservationIds = request.ReservationIds?
                .Where(id => id > 0)
                .Distinct()
                .ToList() ?? new List<int>();

            if (!reservationIds.Any())
            {
                throw new ArgumentException("At least one valid reservation id is required.");
            }

            var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId);
            if (employee == null)
            {
                throw new KeyNotFoundException($"Employee with ID {request.EmployeeId} not found.");
            }

            var updatedReservations = new List<ReservationDto>();

            foreach (var reservationId in reservationIds)
            {
                var reservation = await _reservationRepository.GetReservationByIdAsync(reservationId);
                if (reservation == null)
                {
                    throw new KeyNotFoundException($"Reservation with ID {reservationId} not found.");
                }

                if (reservation.AssignedWaiterId != request.EmployeeId)
                {
                    throw new InvalidOperationException($"Reservation with ID {reservationId} is not assigned to employee with ID {request.EmployeeId}.");
                }

                var updated = await _reservationRepository.UnassignWaiterAsync(reservationId);
                if (updated != null)
                {
                    updatedReservations.Add(updated.ToReservationDto());
                }
            }

            return updatedReservations;
        }
    }
}
