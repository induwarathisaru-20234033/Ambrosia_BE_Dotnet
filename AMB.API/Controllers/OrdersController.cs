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
                    : "Order sent to kitchen successfully"
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
    }
}