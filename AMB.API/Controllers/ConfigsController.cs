using AMB.Application.Dtos;
using AMB.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace AMB.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigsController : ControllerBase
    {
        private readonly IConfigService _configService;

        public ConfigsController(IConfigService configService)
        {
            _configService = configService;
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDto<object>>> CreateServiceRules([FromBody] CreateServiceRulesRequestDto dto)
        {
            await _configService.AddConfigurationsAsync(dto);

            var response = new BaseResponseDto<object>(
                new object(),
                "Configurations saved successfully.");

            return Ok(response);
        }

        [HttpPut]
        public async Task<ActionResult<BaseResponseDto<object>>> UpdateServiceRules([FromBody] UpdateServiceRulesRequestDto dto)
        {
            await _configService.UpdateConfigurationsAsync(dto);

            var response = new BaseResponseDto<object>(
                new object(),
                "Configurations updated successfully.");

            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult<BaseResponseDto<ServiceRulesResponseDto?>>> GetServiceRules()
        {
            var result = await _configService.GetConfigurationsAsync();

            var response = new BaseResponseDto<ServiceRulesResponseDto?>(
                result,
                "Configurations retrieved successfully.");

            return Ok(response);
        }
    }
}
