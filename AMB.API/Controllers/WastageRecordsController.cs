using AMB.API.Attributes;
using AMB.Application.Dtos;
using AMB.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AMB.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableAuthorization]
    public class WastageRecordsController : ControllerBase
    {
        private readonly IWastageRecordService _wastageRecordService;

        public WastageRecordsController(IWastageRecordService wastageRecordService)
        {
            _wastageRecordService = wastageRecordService;
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDto<WastageRecordDto>>> Create([FromBody] CreateWastageRecordDto request)
        {
            var result = await _wastageRecordService.CreateWastageRecordAsync(request);
            var response = new BaseResponseDto<WastageRecordDto>(result, "Wastage record created successfully!");
            return Ok(response);
        }
    }
}
