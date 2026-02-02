namespace AMB.Application.Interfaces.Services
{
    public interface IAuthHelper
    {
        Task<string> CreateUserAsync(string email, string password, string fullName);
    }
}
