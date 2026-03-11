using AMB.Application.Dtos;
using AMB.Application.Interfaces;
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
        public async Task<IActionResult> GetMenuItems()
        {
            var items = await _menuService.GetMenuItems();

            return Ok(items);
        }
    }
}