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

        [HttpGet]
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
                "Calender exclusions retrieved successfully.");

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
                null,
                "Table removed successfully.");

            return Ok(response);
        }
    }
}
