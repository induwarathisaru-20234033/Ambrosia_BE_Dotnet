using AMB.API.Attributes;
using AMB.Application.Dtos;
using AMB.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace AMB.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableAuthorization]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryItemService _inventoryItemService;

        public InventoryController(IInventoryItemService inventoryItemService)
        {
            _inventoryItemService = inventoryItemService;
        }

        [HttpPost("items")]
        public async Task<ActionResult<BaseResponseDto<InventoryItemDto>>> Create([FromBody] CreateInventoryItemRequestDto request)
        {
            var result = await _inventoryItemService.CreateInventoryItemAsync(request);
            var response = new BaseResponseDto<InventoryItemDto>(result, "Inventory item created successfully!");

            return Ok(response);
        }

        [HttpPatch("items/{id:int}")]
        public async Task<ActionResult<BaseResponseDto<InventoryItemDto>>> Update(int id, [FromBody] UpdateInventoryItemRequestDto request)
        {
            request.Id = id;
            var result = await _inventoryItemService.UpdateInventoryItemAsync(request);
            var response = new BaseResponseDto<InventoryItemDto>(result, "Inventory item updated successfully!");

            return Ok(response);
        }

        [HttpGet("items/{id:int}")]
        public async Task<ActionResult<BaseResponseDto<InventoryItemDto>>> GetById(int id)
        {
            if (id <= 0)
            {
                var errorResponse = new BaseResponseDto<InventoryItemDto>(
                    "Invalid id.",
                    new List<string> { "id must be greater than zero." });

                return BadRequest(errorResponse);
            }

            var result = await _inventoryItemService.GetInventoryItemByIdAsync(id);
            var response = new BaseResponseDto<InventoryItemDto>(result, "Inventory item retrieved successfully!");

            return Ok(response);
        }

        [HttpGet("items")]
        public async Task<ActionResult<BaseResponseDto<PaginatedResultDto<InventoryItemDto>>>> GetPaginated([FromQuery] InventoryItemFilterRequestDto request)
        {
            if (request.PageNumber < 1)
            {
                var errorResponse = new BaseResponseDto<PaginatedResultDto<InventoryItemDto>>(
                    "Invalid pagination parameters.",
                    new List<string> { "pageNumber must be greater than zero." });

                return BadRequest(errorResponse);
            }

            var result = await _inventoryItemService.SearchAsync(request);
            var response = new BaseResponseDto<PaginatedResultDto<InventoryItemDto>>(
                result,
                "Inventory items retrieved successfully!");

            return Ok(response);
        }

        [HttpGet("uoms")]
        public async Task<ActionResult<BaseResponseDto<List<UnitOfMeasureDto>>>> GetAllUoMs()
        {
            var result = await _inventoryItemService.GetAllUoMsAsync();
            var response = new BaseResponseDto<List<UnitOfMeasureDto>>(result, "Units of measure retrieved successfully!");

            return Ok(response);
        }

        [HttpGet("currencies")]
        public async Task<ActionResult<BaseResponseDto<List<CurrencyDto>>>> GetAllCurrencies()
        {
            var result = await _inventoryItemService.GetAllCurrenciesAsync();
            var response = new BaseResponseDto<List<CurrencyDto>>(result, "Currencies retrieved successfully!");

            return Ok(response);
        }
    }
}
