using AMB.Application.Dtos;
using AMB.Application.Interfaces.Services;
using AMB.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace AMB.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TablesController : ControllerBase
    {
        private readonly ITableService _tableService;

        public TablesController(ITableService tableService)
        {
            _tableService = tableService;
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDto<TableDto>>> Create([FromBody] CreateTableRequestDto request)
        {
            var result = await _tableService.CreateTableAsync(request);
            var response = new BaseResponseDto<TableDto>(result, "Table created successfully!");
            return Ok(response);
        }

        [HttpPost("floor-map")]
        public async Task<ActionResult<BaseResponseDto<object>>> SaveFloorMap([FromBody] SaveTableFloorMapRequestDto request)
        {
            await _tableService.SaveTableFloorMapAsync(request);

            var response = new BaseResponseDto<object>(new object(), "Table floor map saved successfully!");
            return Ok(response);
        }

        [HttpGet("floor-map")]
        public async Task<ActionResult<BaseResponseDto<GetTableFloorMapResponseDto>>> GetFloorMap()
        {
            var result = await _tableService.GetTableFloorMapAsync();
            var response = new BaseResponseDto<GetTableFloorMapResponseDto>(result, "Table floor map retrieved successfully!");
            return Ok(response);
        }

        [HttpGet("search")]
        public async Task<ActionResult<BaseResponseDto<PaginatedResultDto<TableDto>>>> Search([FromQuery] SearchTableRequestDto request)
        {
            if (request.PageNumber < 1)
            {
                var errorResponse = new BaseResponseDto<PaginatedResultDto<TableDto>>(
                    "Invalid pagination parameters.",
                    new List<string> { "pageNumber must be greater than zero." });

                return BadRequest(errorResponse);
            }

            var result = await _tableService.SearchAsync(request);
            var response = new BaseResponseDto<PaginatedResultDto<TableDto>>(
                result,
                "Tables retrieved successfully.");

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<BaseResponseDto<object>>> Remove(int id)
        {
            if (id <= 0)
            {
                var errorResponse = new BaseResponseDto<object>(
                    "Invalid id.",
                    new List<string> { "id must be greater than zero." });

                return BadRequest(errorResponse);
            }

            await _tableService.RemoveTableAsync(id);

            var response = new BaseResponseDto<object>(
                new object(),
                "Table removed successfully.");

            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult<BaseResponseDto<List<TableDto>>>> GetAll([FromQuery] DateOnly? date = null)
        {
            var result = date.HasValue
                ? await _tableService.GetTablesWithAllocationsAsync(date)
                : await _tableService.GetAllAsync();

            var response = new BaseResponseDto<List<TableDto>>(result, "Tables retrieved successfully");
            return Ok(response);
        }
    }
}
