using AMB.Application.Dtos;
using AMB.Application.Interfaces.Services;
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

        [HttpGet]
        public async Task<ActionResult<BaseResponseDto<PaginatedResultDto<CalenderExclusionDto>>>> GetAll(
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

    }
}
