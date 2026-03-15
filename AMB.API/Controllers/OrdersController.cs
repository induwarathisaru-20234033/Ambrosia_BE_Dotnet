using AMB.API.Attributes;
using AMB.Application.Dtos;
using AMB.Application.Interfaces.Services;
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
                result.OrderStatus == "Draft"
                    ? "Order saved as draft successfully"
                    : "Order sent to KDS successfully"
            );

            return Ok(response);
        }

        // Search menu items by name 
        [HttpGet("menu-items/search")]
        public async Task<ActionResult<BaseResponseDto<List<MenuItemDto>>>> SearchMenuItems([FromQuery] string term)
        {
            var result = await _orderService.SearchMenuItemsAsync(term);

            var response = new BaseResponseDto<List<MenuItemDto>>(
                result,
                "Search results retrieved successfully"
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
    }
}