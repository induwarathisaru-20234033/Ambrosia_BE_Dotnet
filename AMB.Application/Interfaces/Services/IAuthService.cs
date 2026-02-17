using AMB.Application.Dtos;

namespace AMB.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthTokenResponseDto> LoginAsync(AuthLoginRequestDto request);
        Task<AuthTokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request);
        Task<AuthUserProfileDto> GetUserProfileAsync(string accessToken);
        Task<LogoutResponseDto> BuildLogoutUrlAsync(LogoutRequestDto request);
        Task UpdatePasswordAsync(UpdatePasswordRequestDto request);
    }
}
