using AMB.Application.Dtos;
using AMB.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMB.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationsController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDto<ReservationDto>>> CreateReservation([FromBody] CreateReservationRequestDto request)
        {
            var reservation = await _reservationService.CreateReservationAsync(request);
            var response = new BaseResponseDto<ReservationDto>(reservation, "Reservation created successfully");
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BaseResponseDto<ReservationDto>>> GetReservation(int id)
        {
            var reservation = await _reservationService.GetReservationByIdAsync(id);
            if (reservation == null)
            {
                var errorResponse = new BaseResponseDto<ReservationDto>(
                    "Reservation not found",
                    new List<string> { $"No reservation found with ID {id}" });
                return NotFound(errorResponse);
            }

            var response = new BaseResponseDto<ReservationDto>(reservation, "Reservation retrieved successfully");
            return Ok(response);
        }

        [HttpGet("code/{reservationCode}")]
        public async Task<ActionResult<BaseResponseDto<ReservationDto>>> GetReservationByCode(string reservationCode)
        {
            var reservation = await _reservationService.GetReservationByCodeAsync(reservationCode);
            if (reservation == null)
            {
                var errorResponse = new BaseResponseDto<ReservationDto>(
                    "Reservation not found",
                    new List<string> { $"No reservation found with code {reservationCode}" });
                return NotFound(errorResponse);
            }

            var response = new BaseResponseDto<ReservationDto>(reservation, "Reservation retrieved successfully");
            return Ok(response);
        }

        [HttpGet("by-date/{date}")]
        public async Task<ActionResult<BaseResponseDto<List<ReservationDto>>>> GetReservationsByDate(string date)
        {
            if (!DateOnly.TryParse(date, out var parsedDate))
            {
                var errorResponse = new BaseResponseDto<List<ReservationDto>>(
                    "Invalid date format. Use YYYY-MM-DD",
                    new List<string> { "Date must be in format YYYY-MM-DD" });
                return BadRequest(errorResponse);
            }

            var reservations = await _reservationService.GetReservationsByDateAsync(parsedDate);
            var response = new BaseResponseDto<List<ReservationDto>>(
                reservations,
                $"Retrieved {reservations.Count} reservations for {date}");
            return Ok(response);
        }

        [HttpGet("availability")]
        public async Task<ActionResult<BaseResponseDto<ReservationAvailabilityDto>>> CheckAvailability(
            [FromQuery] int bookingSlotId,
            [FromQuery] int tableId,
            [FromQuery] string date)
        {
            if (!DateOnly.TryParse(date, out var parsedDate))
            {
                var errorResponse = new BaseResponseDto<ReservationAvailabilityDto>(
                    "Invalid date format. Use YYYY-MM-DD",
                    new List<string> { "Date must be in format YYYY-MM-DD" });
                return BadRequest(errorResponse);
            }

            var availability = await _reservationService.CheckAvailabilityAsync(bookingSlotId, tableId, parsedDate);
            var message = availability.IsSlotAvailable && availability.IsTableAvailable
                ? "Slot and table are available"
                : $"Not available. Slot has {availability.ExistingReservationsForSlot} reservations, " +
                  $"Table has {availability.ExistingReservationsForTable} reservations in this time slot";

            var response = new BaseResponseDto<ReservationAvailabilityDto>(availability, message);
            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult<BaseResponseDto<PagedResponseDto<ReservationDto>>>> GetReservations([FromQuery] ReservationFilterRequestDto filter)
        {
            var result = await _reservationService.GetReservationsPagedAsync(filter);
            return Ok(new BaseResponseDto<PagedResponseDto<ReservationDto>>(
                result,
                "Reservations retrieved successfully"
            ));
        }

        [HttpGet("slot/{bookingSlotId}/date/{date}")]
        public async Task<ActionResult<BaseResponseDto<List<ReservationDto>>>> GetBookingSlotReservations(
            int bookingSlotId,
            string date)
        {
            if (!DateOnly.TryParse(date, out var parsedDate))
            {
                var errorResponse = new BaseResponseDto<List<ReservationDto>>(
                    "Invalid date format. Use YYYY-MM-DD",
                    new List<string> { "Date must be in format YYYY-MM-DD" });
                return BadRequest(errorResponse);
            }

            var reservations = await _reservationService.GetBookingSlotReservationsAsync(bookingSlotId, parsedDate);
            var response = new BaseResponseDto<List<ReservationDto>>(
                reservations,
                $"Found {reservations.Count} reservations for this booking slot");
            return Ok(response);
        }

        [HttpGet("table/{tableId}/date/{date}")]
        public async Task<ActionResult<BaseResponseDto<List<ReservationDto>>>> GetTableReservations(
            int tableId,
            string date)
        {
            if (!DateOnly.TryParse(date, out var parsedDate))
            {
                var errorResponse = new BaseResponseDto<List<ReservationDto>>(
                    "Invalid date format. Use YYYY-MM-DD",
                    new List<string> { "Date must be in format YYYY-MM-DD" });
                return BadRequest(errorResponse);
            }

            var reservations = await _reservationService.GetTableReservationsAsync(tableId, parsedDate);
            var response = new BaseResponseDto<List<ReservationDto>>(
                reservations,
                $"Found {reservations.Count} reservations for this table");
            return Ok(response);
        }

        [HttpPatch("{id}/cancel")]
        public async Task<ActionResult<BaseResponseDto<ReservationDto>>> CancelReservation(int id)
        {
            var reservation = await _reservationService.CancelReservationAsync(id);
            if (reservation == null)
            {
                var errorResponse = new BaseResponseDto<ReservationDto>(
                    "Reservation not found",
                    new List<string> { $"No reservation found with ID {id}" });
                return NotFound(errorResponse);
            }

            var response = new BaseResponseDto<ReservationDto>(reservation, "Reservation cancelled successfully");
            return Ok(response);
        }

        [HttpPatch("{id}/arrived")]
        public async Task<ActionResult<BaseResponseDto<ReservationDto>>> MarkAsArrived(int id)
        {
            var reservation = await _reservationService.MarkReservationAsArrivedAsync(id);
            if (reservation == null)
            {
                var errorResponse = new BaseResponseDto<ReservationDto>(
                    "Reservation not found",
                    new List<string> { $"No reservation found with ID {id}" });
                return NotFound(errorResponse);
            }

            var response = new BaseResponseDto<ReservationDto>(reservation, "Reservation marked as arrived");
            return Ok(response);
        }

        [HttpPatch("{id}/no-show")]
        public async Task<ActionResult<BaseResponseDto<ReservationDto>>> MarkAsNoShow(int id)
        {
            var reservation = await _reservationService.MarkReservationAsNoShowAsync(id);
            if (reservation == null)
            {
                var errorResponse = new BaseResponseDto<ReservationDto>(
                    "Reservation not found",
                    new List<string> { $"No reservation found with ID {id}" });
                return NotFound(errorResponse);
            }

            var response = new BaseResponseDto<ReservationDto>(reservation, "Reservation marked as no-show");
            return Ok(response);
        }

        [HttpPatch("assign-waiter")]
        public async Task<ActionResult<BaseResponseDto<List<ReservationDto>>>> AssignWaiter([FromBody] AssignWaiterRequestDto dto)
        {
            var reservations = await _reservationService.AssignWaiterAsync(dto);
            var response = new BaseResponseDto<List<ReservationDto>>(reservations, "Waiter assigned successfully");
            return Ok(response);
        }

        [HttpPatch("unassign-waiter")]
        public async Task<ActionResult<BaseResponseDto<List<ReservationDto>>>> UnassignWaiter([FromBody] AssignWaiterRequestDto dto)
        {
            var reservations = await _reservationService.UnassignWaiterAsync(dto);
            var response = new BaseResponseDto<List<ReservationDto>>(reservations, "Waiter unassigned successfully");
            return Ok(response);
        }
    }
}
