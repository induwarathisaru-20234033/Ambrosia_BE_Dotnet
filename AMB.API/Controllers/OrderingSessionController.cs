using AMB.Application.Dtos;
using AMB.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMB.API.Controllers
{
    [ApiController]
    [Route("api/ordering")]
    public class OrderingSessionController : ControllerBase
    {
        private readonly IOrderingSessionService _orderingSessionService;
        private readonly IOrderService _orderService;

        public OrderingSessionController(
            IOrderingSessionService orderingSessionService,
            IOrderService orderService)
        {
            _orderingSessionService = orderingSessionService;
            _orderService = orderService;
        }

        [HttpGet("check-table/{tableGuid}")]
        [EnableRateLimiting("QrScanLimit")]
        public async Task<ActionResult<CheckTableResponseDto>> CheckTableOccupancy(Guid tableGuid)
        {
            var response = await _orderingSessionService.CheckTableOccupancyAsync(tableGuid);

            if (response == null)
            {
                return NotFound(new { Message = "Table is not occupied or reservation has expired." });
            }

            return Ok(response);
        }

        [HttpPost("confirm-session")]
        [EnableRateLimiting("QrScanLimit")]
        public async Task<ActionResult<ConfirmSessionResponseDto>> ConfirmSession([FromBody] ConfirmSessionRequestDto request)
        {
            if (request == null)
            {
                return BadRequest(new { Message = "Request body is required." });
            }

            var response = await _orderingSessionService.ConfirmSessionAsync(request);

            if (response == null)
            {
                return NotFound(new { Message = "Unable to confirm session. Table may not be occupied, reservation not arrived, or confirmation failed." });
            }

            return Ok(response);
        }

        [HttpPost("place-order")]
        [Authorize(Policy = "GuestPolicy")]
        public async Task<ActionResult<BaseResponseDto<OrderResponseDto>>> CreateOrder([FromBody] CreateOrderRequestDto request)
        {
            if (request == null)
            {
                return BadRequest(new BaseResponseDto<OrderResponseDto>("Request body is required.", new List<string> { "Create order payload is required." }));
            }

            var reservationClaim = User.FindFirst("res_id")?.Value;
            if (!string.IsNullOrWhiteSpace(reservationClaim) && int.TryParse(reservationClaim, out var reservationId))
            {
                request.ReservationId = reservationId;
            }

            var order = await _orderService.CreateOrderAsync(request);

            var response = new BaseResponseDto<OrderResponseDto>(order, "Order placed successfully.");
            return Ok(response);
        }
    }
}
