using AMB.Application.Dtos;
using AMB.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        [AllowAnonymous]
        public async Task<ActionResult<BaseResponseDto<AuthTokenResponseDto>>> Login([FromBody] AuthLoginRequestDto request)
        {
            var result = await _authService.LoginAsync(request);
            var response = new BaseResponseDto<AuthTokenResponseDto>(result, "Login successful.");

            return Ok(response);
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<ActionResult<BaseResponseDto<AuthTokenResponseDto>>> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            var result = await _authService.RefreshTokenAsync(request);
            var response = new BaseResponseDto<AuthTokenResponseDto>(result, "Token refreshed.");

            return Ok(response);
        }

        [HttpPost("logout")]
        [AllowAnonymous]
        public async Task<ActionResult<BaseResponseDto<LogoutResponseDto>>> Logout([FromBody] LogoutRequestDto request)
        {
            var result = await _authService.BuildLogoutUrlAsync(request);
            var response = new BaseResponseDto<LogoutResponseDto>(result, "Logout URL generated.");

            return Ok(response);
        }

        [HttpGet("user-profile")]
        [Authorize]
        public async Task<ActionResult<BaseResponseDto<AuthUserProfileDto>>> UserProfile()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            accessToken ??= ExtractBearerToken(Request.Headers.Authorization);

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return Unauthorized("Access token is required.");
            }

            var result = await _authService.GetUserProfileAsync(accessToken);
            var response = new BaseResponseDto<AuthUserProfileDto>(result, "Profile loaded.");

            return Ok(response);
        }

        private static string? ExtractBearerToken(string? authorizationHeader)
        {
            if (string.IsNullOrWhiteSpace(authorizationHeader))
            {
                return null;
            }

            const string bearerPrefix = "Bearer ";
            if (authorizationHeader.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return authorizationHeader.Substring(bearerPrefix.Length).Trim();
            }

            return authorizationHeader.Trim();
        }
    }
}
