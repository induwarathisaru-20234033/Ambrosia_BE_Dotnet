using AMB.API.Attributes;
using AMB.Application.Dtos;
using AMB.Application.Interfaces.Services;
using AMB.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace AMB.API.Controllers
{
    [ApiController]
    [Route("api/orders")]
    [EnableAuthorization]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        // Create a new order (draft or fire)
        [HttpPost]
        public async Task<ActionResult<BaseResponseDto<OrderResponseDto>>> CreateOrder([FromBody] CreateOrderRequestDto dto)
        {
            var result = await _orderService.CreateOrderAsync(dto);

            var response = new BaseResponseDto<OrderResponseDto>(
                result,
                result.OrderStatus == OrderStatus.Draft
                    ? "Order saved as draft successfully"
                    : "Order sent to KDS successfully"
            );

            return Ok(response);
        }

        // Get order by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<BaseResponseDto<OrderResponseDto>>> GetOrderById(int id)
        {
            var result = await _orderService.GetOrderByIdAsync(id);

            var response = new BaseResponseDto<OrderResponseDto>(
                result,
                "Order retrieved successfully"
            );

            return Ok(response);
        }

        // Send a draft order to KDS
        [HttpPost("{id}/send-to-kds")]
        public async Task<ActionResult<BaseResponseDto<OrderResponseDto>>> SendDraftToKds(
            int id,
            [FromBody] SendOrderToKdsDto dto)
        {
            if (id != dto.OrderId)
            {
                return BadRequest(new BaseResponseDto<OrderResponseDto>(
                    "ID mismatch",
                    new List<string> { "URL ID does not match request body ID" }
                ));
            }

            var result = await _orderService.SendDraftToKdsAsync(dto);

            var response = new BaseResponseDto<OrderResponseDto>(
                result,
                "Order sent to KDS successfully"
            );

            return Ok(response);
        }

        // Update order status (Preparing, On Hold, Ready, Served, Cancelled)
        [HttpPut("{id}/status")]
        public async Task<ActionResult<BaseResponseDto<OrderResponseDto>>> UpdateOrderStatus(
            int id,
            [FromBody] UpdateOrderStatusDto dto)
        {
            if (id != dto.OrderId)
            {
                return BadRequest(new BaseResponseDto<OrderResponseDto>(
                    "ID mismatch",
                    new List<string> { "URL ID does not match request body ID" }
                ));
            }

            var result = await _orderService.UpdateOrderStatusAsync(dto);

            var response = new BaseResponseDto<OrderResponseDto>(
                result,
                $"Order status updated to {result.OrderStatus}"
            );

            return Ok(response);
        }

        // Get all orders by status (for filtering)
        [HttpGet("status/{status}")]
        public async Task<ActionResult<BaseResponseDto<List<OrderResponseDto>>>> GetOrdersByStatus(string status)
        {
            if (!Enum.TryParse<OrderStatus>(status, true, out var orderStatus))
            {
                return BadRequest(new BaseResponseDto<List<OrderResponseDto>>(
                    "Invalid status value",
                    new List<string> { $"Status must be one of: {string.Join(", ", Enum.GetNames<OrderStatus>())}" }
                ));
            }

            var result = await _orderService.GetOrdersByStatusAsync(orderStatus);

            var response = new BaseResponseDto<List<OrderResponseDto>>(
                result,
                $"Orders with status '{status}' retrieved successfully"
            );

            return Ok(response);
        }

        // Get all kitchen orders (Sent to KDS, Preparing, On Hold)
        [HttpGet("kitchen")]
        public async Task<ActionResult<BaseResponseDto<List<OrderResponseDto>>>> GetKitchenOrders()
        {
            var result = await _orderService.GetKitchenOrdersAsync();

            var response = new BaseResponseDto<List<OrderResponseDto>>(
                result,
                "Kitchen orders retrieved successfully"
            );

            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult<BaseResponseDto<PagedResponseDto<OrderResponseDto>>>> SearchOrders(
            [FromQuery] SearchOrderRequestDto dto)
        {
            var result = await _orderService.SearchOrdersAsync(dto);

            var response = new BaseResponseDto<PagedResponseDto<OrderResponseDto>>(
                result,
                "Orders retrieved successfully"
            );

            return Ok(response);
        }

        // Update a draft order (add/remove items)
        [HttpPut("{id}/items")]
        public async Task<ActionResult<BaseResponseDto<OrderResponseDto>>> UpdateDraftOrder(
            int id,
            [FromBody] UpdateDraftOrderDto dto)
        {
            if (id != dto.OrderId)
            {
                return BadRequest(new BaseResponseDto<OrderResponseDto>(
                    "ID mismatch",
                    new List<string> { "URL ID does not match request body ID" }
                ));
            }

            var result = await _orderService.UpdateDraftOrderAsync(dto);

            var response = new BaseResponseDto<OrderResponseDto>(
                result,
                "Draft order updated successfully"
            );

            return NoContent();
        }

        // Remove an item from a draft order
        [HttpDelete("{orderId}/items/{menuItemId}")]
        public async Task<ActionResult<BaseResponseDto<OrderResponseDto>>> RemoveItemFromOrder(
            int orderId,
            int menuItemId)
        {
            try
            {
                var result = await _orderService.RemoveItemFromOrderAsync(orderId, menuItemId);

                var response = new BaseResponseDto<OrderResponseDto>(
                    result,
                    "Item removed from order successfully"
                );

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new BaseResponseDto<OrderResponseDto>(
                    ex.Message,
                    new List<string> { ex.Message }
                ));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new BaseResponseDto<OrderResponseDto>(
                    ex.Message,
                    new List<string> { ex.Message }
                )  );
            }
        }
    }
}