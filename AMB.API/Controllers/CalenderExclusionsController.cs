using AMB.Application.Dtos;
using AMB.Application.Interfaces.Services;
using AMB.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMB.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalenderExclusionsController : ControllerBase
    {
        private readonly ICalendarExclusionService _calendarExclusionService;

        public CalenderExclusionsController(ICalendarExclusionService calendarExclusionService)
        {
            _calendarExclusionService = calendarExclusionService;
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDto<CalenderExclusionDto>>> Create(
            [FromBody] CreateCalenderExclusionRequestDto request)
        {
            var result = await _calendarExclusionService.CreateCalenderExclusionAsync(request);
            var response = new BaseResponseDto<CalenderExclusionDto>(
                result,
                "Calender exclusion created successfully.");

            return Ok(response);
        }

        [HttpGet("search")]
        public async Task<ActionResult<BaseResponseDto<PaginatedResultDto<CalenderExclusionDto>>>> Search(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1)
            {
                var errorResponse = new BaseResponseDto<PaginatedResultDto<CalenderExclusionDto>>(
                    "Invalid pagination parameters.",
                    new List<string> { "pageNumber must be greater than zero." });

                return BadRequest(errorResponse);
            }

            var result = await _calendarExclusionService.GetPaginatedExclusionsAsync(pageNumber, pageSize);
            var response = new BaseResponseDto<PaginatedResultDto<CalenderExclusionDto>>(
                result,
                "Calender exclusions retrieved successfully.");

            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult<BaseResponseDto<List<CalenderExclusionDto>>>> GetAll()
        {
            var result = await _calendarExclusionService.GetAllAsync();
            var response = new BaseResponseDto<List<CalenderExclusionDto>>(
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

            await _calendarExclusionService.RemoveExclusionAsync(id);

            var response = new BaseResponseDto<object>(
                null,
                "Calender Exclusion removed successfully.");

            return Ok(response);
        }

    }
}
