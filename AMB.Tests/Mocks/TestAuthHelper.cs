using AMB.Application.Interfaces.Services;

namespace AMB.Tests.Mocks
{
    internal sealed class TestAuthHelper : IAuthHelper
    {
        private readonly string _userId;

        public TestAuthHelper(string userId)
        {
            _userId = userId;
        }

        public string? LastUsername { get; private set; }
        public string? LastPassword { get; private set; }
        public string? LastFullName { get; private set; }

        public Task<string> CreateUserAsync(string email, string password, string fullName)
        {
            LastUsername = email;
            LastPassword = password;
            LastFullName = fullName;
            return Task.FromResult(_userId);
        }
    }
}
