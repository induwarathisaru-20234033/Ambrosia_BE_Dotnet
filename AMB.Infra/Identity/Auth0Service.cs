using AMB.Application.Interfaces.Services;
using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace AMB.Infra.Identity
{
    public class Auth0Service : IAuthHelper
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public Auth0Service(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> CreateUserAsync(string email, string password, string fullName)
        {
            var token = await GetManagementApiTokenAsync();

            var client = new ManagementApiClient(token, new Uri($"https://{_configuration["Authentication:Domain"]}/api/v2/"));

            var request = new UserCreateRequest
            {
                Email = email,
                Password = password,
                FullName = fullName,
                Connection = "Username-Password-Authentication", // Default Auth0 DB connection
                EmailVerified = true // Auto-verify to allow immediate login
            };

            var user = await client.Users.CreateAsync(request);

            return user.UserId;
        }

        private async Task<string> GetManagementApiTokenAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var domain = _configuration["Authentication:Domain"];

            var request = new HttpRequestMessage(HttpMethod.Post, $"https://{domain}/oauth/token");
            var payload = new
            {
                client_id = _configuration["Authentication:ClientId"],
                client_secret = _configuration["Authentication:ClientSecret"],
                audience = $"https://{domain}/api/v2/",
                grant_type = "client_credentials"
            };

            request.Content = JsonContent.Create(payload);
            
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
            return json.GetProperty("access_token").GetString() ?? "";
        }
    }
}
