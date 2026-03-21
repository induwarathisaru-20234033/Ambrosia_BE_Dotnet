using AMB.API.Attributes;
using AMB.Application.Dtos;
using AMB.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace AMB.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableAuthorization]
    public class GoodsIssueController : ControllerBase
    {
        private readonly IGoodsIssueService _goodsIssueService;

        public GoodsIssueController(IGoodsIssueService goodsIssueService)
        {
            _goodsIssueService = goodsIssueService;
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDto<GoodIssueNoteDto>>> Create([FromBody] CreateGoodIssueNoteDto request)
        {
            var result = await _goodsIssueService.CreateGoodIssueNoteAsync(request);
            var response = new BaseResponseDto<GoodIssueNoteDto>(result, "Good issue note created successfully!");
            return Ok(response);
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult<BaseResponseDto<GoodIssueNoteDto>>> Update(int id, [FromBody] UpdateGoodIssueNoteDto request)
        {
            request.Id = id;
            var result = await _goodsIssueService.UpdateGoodIssueNoteAsync(request);
            var response = new BaseResponseDto<GoodIssueNoteDto>(result, "Good issue note updated successfully!");
            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult<BaseResponseDto<PaginatedResultDto<GoodIssueNoteDto>>>> GetPaginated([FromQuery] GoodIssueNoteFilterRequestDto request)
        {
            if (request.PageNumber < 1)
            {
                var errorResponse = new BaseResponseDto<PaginatedResultDto<GoodIssueNoteDto>>(
                    "Invalid pagination parameters.",
                    new List<string> { "pageNumber must be greater than zero." });

                return BadRequest(errorResponse);
            }

            if (request.IssuedDateFrom.HasValue
                && request.IssuedDateTo.HasValue
                && request.IssuedDateFrom.Value > request.IssuedDateTo.Value)
            {
                var errorResponse = new BaseResponseDto<PaginatedResultDto<GoodIssueNoteDto>>(
                    "Invalid date range.",
                    new List<string> { "issuedDateFrom must be less than or equal to issuedDateTo." });

                return BadRequest(errorResponse);
            }

            var result = await _goodsIssueService.GetGoodIssueNotesPagedAsync(request);
            var response = new BaseResponseDto<PaginatedResultDto<GoodIssueNoteDto>>(
                result,
                "Good issue notes retrieved successfully!");

            return Ok(response);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<BaseResponseDto<GoodIssueNoteDto>>> GetById(int id)
        {
            if (id <= 0)
            {
                var errorResponse = new BaseResponseDto<GoodIssueNoteDto>(
                    "Invalid id.",
                    new List<string> { "id must be greater than zero." });

                return BadRequest(errorResponse);
            }

            var result = await _goodsIssueService.GetGoodIssueNoteByIdAsync(id);
            var response = new BaseResponseDto<GoodIssueNoteDto>(result, "Good issue note retrieved successfully!");

            return Ok(response);
        }
    }
}
