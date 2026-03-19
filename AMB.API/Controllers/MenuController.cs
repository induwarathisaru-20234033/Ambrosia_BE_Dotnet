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
        public async Task<IActionResult> AddMenuItem([FromBody] CreateMenuItemDto dto)
        {
            var createdItem = await _menuService.AddMenuItem(dto);

            return CreatedAtAction(nameof(GetMenuItems), new { id = createdItem.Id }, createdItem);
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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMenuItem(int id, [FromBody] UpdateMenuItemDto dto)
        {
            try
            {
                var updatedItem = await _menuService.UpdateMenuItem(id, dto);
                return Ok(updatedItem);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}