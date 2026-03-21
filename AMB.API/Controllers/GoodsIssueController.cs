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
    }
}
