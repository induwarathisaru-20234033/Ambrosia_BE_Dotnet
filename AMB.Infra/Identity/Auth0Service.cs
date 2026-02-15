using AMB.Application.Interfaces.Services;
using AMB.Domain.Exceptions;
using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using AMB.Application.Dtos;
using System.Net;

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

            try
            {
                var user = await client.Users.CreateAsync(request);
                return user.UserId;
            }
            catch (Auth0.Core.Exceptions.ApiException apiException)
            {
                throw new Auth0Exception(
                    message: "Failed to create user in Auth0.",
                    error: apiException.Message,
                    errorDescription: apiException?.InnerException?.Message,
                    statusCode: (int)HttpStatusCode.BadRequest
                );
            }
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
            
            if (!response.IsSuccessStatusCode)
            {
                await ThrowAuth0ExceptionAsync(response, "Failed to obtain Auth0 Management API token.");
            }

            var json = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
            return json.GetProperty("access_token").GetString() ?? "";
        }

        private async Task ThrowAuth0ExceptionAsync(HttpResponseMessage response, string defaultMessage)
        {
            var statusCode = (int)response.StatusCode;
            Auth0ErrorResponseDto? errorResponse = null;

            try
            {
                errorResponse = await response.Content.ReadFromJsonAsync<Auth0ErrorResponseDto>();
            }
            catch
            {
                // Ignore deserialization errors
            }

            var error = errorResponse?.Error ?? "auth0_error";
            var errorDescription = errorResponse?.ErrorDescription 
                ?? errorResponse?.Message 
                ?? await response.Content.ReadAsStringAsync() 
                ?? defaultMessage;

            throw new Auth0Exception(
                message: defaultMessage,
                error: error,
                errorDescription: errorDescription,
                statusCode: statusCode
            );
        }
    }
}
