namespace AMB.Application.Interfaces.Services
{
    public interface IAuthHelper
    {
        Task<string> CreateUserAsync(string email, string password, string fullName);
        Task DeleteUserAsync(string authId);
        Task UpdatePasswordAsync(string authUserId, string newPassword);
    }
}
