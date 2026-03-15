using AMB.Application.Dtos;
using AMB.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace AMB.API.Controllers
{
    [ApiController]
    [Route("api/menu")]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        [HttpPost]
        public async Task<IActionResult> AddMenuItem(MenuItemDto dto)
        {
            await _menuService.AddMenuItem(dto);

            return Ok("Menu item added");
        }

        [HttpGet]
        public async Task<IActionResult> GetMenuItems(
                [FromQuery] string? category,
                [FromQuery] string? name,
                [FromQuery] bool? isAvailable
            )
        {
            var items = await _menuService.GetMenuItems(category, name, isAvailable);

            return Ok(items);
        }
    }
}