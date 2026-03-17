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
    }
}
