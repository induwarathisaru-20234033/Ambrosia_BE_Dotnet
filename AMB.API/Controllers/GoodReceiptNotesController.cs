using AMB.API.Attributes;
using AMB.Application.Dtos;
using AMB.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace AMB.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableAuthorization]
    public class GoodReceiptNotesController : ControllerBase
    {
        private readonly IGoodReceiptNoteService _goodReceiptNoteService;

        public GoodReceiptNotesController(IGoodReceiptNoteService goodReceiptNoteService)
        {
            _goodReceiptNoteService = goodReceiptNoteService;
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDto<GoodReceiptNoteDto>>> Create([FromBody] CreateGoodReceiptNoteDto request)
        {
            var result = await _goodReceiptNoteService.CreateGoodReceiptNoteAsync(request);
            var response = new BaseResponseDto<GoodReceiptNoteDto>(result, "GRN created successfully!");
            return Ok(response);
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult<BaseResponseDto<GoodReceiptNoteDto>>> Update(int id, [FromBody] UpdateGoodReceiptNoteDto request)
        {
            request.Id = id;
            var result = await _goodReceiptNoteService.UpdateGoodReceiptNoteAsync(request);
            var response = new BaseResponseDto<GoodReceiptNoteDto>(result, "GRN updated successfully!");
            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult<BaseResponseDto<PaginatedResultDto<GoodReceiptNoteDto>>>> GetPaginated([FromQuery] GoodReceiptNoteFilterRequestDto request)
        {
            if (request.PageNumber < 1)
            {
                var errorResponse = new BaseResponseDto<PaginatedResultDto<GoodReceiptNoteDto>>(
                    "Invalid pagination parameters.",
                    new List<string> { "pageNumber must be greater than zero." });

                return BadRequest(errorResponse);
            }

            if (request.ReceivedDateFrom.HasValue
                && request.ReceivedDateTo.HasValue
                && request.ReceivedDateFrom.Value > request.ReceivedDateTo.Value)
            {
                var errorResponse = new BaseResponseDto<PaginatedResultDto<GoodReceiptNoteDto>>(
                    "Invalid date range.",
                    new List<string> { "receivedDateFrom must be less than or equal to receivedDateTo." });

                return BadRequest(errorResponse);
            }

            var result = await _goodReceiptNoteService.GetGoodReceiptNotesPagedAsync(request);
            var response = new BaseResponseDto<PaginatedResultDto<GoodReceiptNoteDto>>(
                result,
                "GRNs retrieved successfully!");

            return Ok(response);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<BaseResponseDto<GoodReceiptNoteDto>>> GetById(int id)
        {
            if (id <= 0)
            {
                var errorResponse = new BaseResponseDto<GoodReceiptNoteDto>(
                    "Invalid id.",
                    new List<string> { "id must be greater than zero." });

                return BadRequest(errorResponse);
            }

            var result = await _goodReceiptNoteService.GetGoodReceiptNoteByIdAsync(id);
            var response = new BaseResponseDto<GoodReceiptNoteDto>(result, "GRN retrieved successfully!");

            return Ok(response);
        }
    }
}
