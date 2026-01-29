using AMB.Application.Dtos;

namespace AMB.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(string authUserId);
    }
}
