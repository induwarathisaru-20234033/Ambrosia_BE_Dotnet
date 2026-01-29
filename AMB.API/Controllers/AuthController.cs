using AMB.API.Attributes;
using AMB.Application.Dtos;
using AMB.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AMB.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [EnableAuthorization]
        public async Task<ActionResult<BaseResponseDto<LoginResponseDto>>> Login()
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User?.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized("Invalid access token.");
            }

            var result = await _authService.LoginAsync(userId);

            var response = new BaseResponseDto<LoginResponseDto>(result, "Login successful.");

            return Ok(response);
        }
    }
}
