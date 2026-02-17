using System.Net.Http.Headers;
using System.Net.Http.Json;
using AMB.Application.Dtos;
using AMB.Application.Interfaces.Repositories;
using AMB.Application.Interfaces.Services;
using AMB.Domain.Exceptions;
using AMB.Domain.Enums;
using Microsoft.Extensions.Configuration;

namespace AMB.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IAuthHelper _authHelper;
        private readonly IEmployeeRepository _employeeRepository;

        public AuthService(IConfiguration configuration, IHttpClientFactory httpClientFactory, IAuthHelper authHelper, IEmployeeRepository employeeRepository)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _authHelper = authHelper;
            _employeeRepository = employeeRepository;
        }

        /// <summary>
        /// Exchanges a username/password for Auth0 tokens using the password grant.
        /// </summary>
        /// <param name="request">Login request containing username and password.</param>
        /// <returns>Auth0 token response with access token and optional refresh token.</returns>
        public async Task<AuthTokenResponseDto> LoginAsync(AuthLoginRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                throw new ArgumentException("Username and password are required.");
            }

            var employee = await _employeeRepository.GetByUsernameAsync(request.Username);
            if (employee == null)
            {
                throw new UnauthorizedAccessException("We could not find an active account for this username. Please contact support if you believe this is a mistake.");
            }

            if (employee.Status != (int)EntityStatus.Active)
            {
                throw new UnauthorizedAccessException("Your account is not active yet. Please contact support to activate your access.");
            }

            var payload = new
            {
                grant_type = "password",
                username = request.Username,
                password = request.Password,
                audience = GetRequiredConfig("Authentication:Audience"),
                client_id = GetRequiredConfig("Authentication:ClientId"),
                client_secret = GetRequiredConfig("Authentication:ClientSecret"),
                scope = "openid profile email offline_access"
            };

            return await RequestTokenAsync(payload, "Login failed.");
        }

        /// <summary>
        /// Exchanges a refresh token for a new access token.
        /// </summary>
        /// <param name="request">Request containing the refresh token.</param>
        /// <returns>Auth0 token response with a new access token.</returns>
        public async Task<AuthTokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                throw new ArgumentException("Refresh token is required.");
            }

            var payload = new
            {
                grant_type = "refresh_token",
                refresh_token = request.RefreshToken,
                client_id = GetRequiredConfig("Authentication:ClientId"),
                client_secret = GetRequiredConfig("Authentication:ClientSecret")
            };

            return await RequestTokenAsync(payload);
        }

        /// <summary>
        /// Fetches the Auth0 user profile using a bearer access token.
        /// </summary>
        /// <param name="accessToken">Bearer access token issued by Auth0.</param>
        /// <returns>User profile data from the Auth0 /userinfo endpoint.</returns>
        public async Task<AuthUserProfileDto> GetUserProfileAsync(string accessToken)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new ArgumentException("Access token is required.");
            }

            var domain = GetRequiredConfig("Authentication:Domain");
            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://{domain}/userinfo");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.SendAsync(request);
            
            if (!response.IsSuccessStatusCode)
            {
                await ThrowAuth0ExceptionAsync(response, "Failed to fetch user profile from Auth0.");
            }

            var profile = await response.Content.ReadFromJsonAsync<AuthUserProfileDto>();
            if (profile == null)
            {
                throw new InvalidOperationException("Failed to read user profile response.");
            }

            return profile;
        }

        /// <summary>
        /// Builds the Auth0 logout URL for client-side sign-out.
        /// </summary>
        /// <param name="request">Request containing optional return URL.</param>
        /// <returns>Logout URL to redirect the client.</returns>
        public Task<LogoutResponseDto> BuildLogoutUrlAsync(LogoutRequestDto request)
        {
            var domain = GetRequiredConfig("Authentication:Domain");
            var clientId = GetRequiredConfig("Authentication:ClientId");
            var returnTo = request.ReturnTo ?? _configuration["Authentication:LogoutReturnTo"];

            var logoutUrl = string.IsNullOrWhiteSpace(returnTo)
                ? $"https://{domain}/v2/logout?client_id={Uri.EscapeDataString(clientId)}"
                : $"https://{domain}/v2/logout?client_id={Uri.EscapeDataString(clientId)}&returnTo={Uri.EscapeDataString(returnTo)}";

            return Task.FromResult(new LogoutResponseDto { LogoutUrl = logoutUrl });
        }

        /// <summary>
        /// Updates the password for a user identified by username.
        /// </summary>
        /// <param name="request">Request containing username and new password.</param>
        /// <returns>Completed task when password is updated.</returns>
        public async Task UpdatePasswordAsync(UpdatePasswordRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Username))
            {
                throw new ArgumentException("Username is required.");
            }

            if (string.IsNullOrWhiteSpace(request.NewPassword))
            {
                throw new ArgumentException("New password is required.");
            }

            var employee = await _employeeRepository.GetByUsernameAsync(request.Username);
            if (employee == null)
            {
                throw new KeyNotFoundException($"Employee with username '{request.Username}' not found.");
            }

            if (string.IsNullOrWhiteSpace(employee.UserId))
            {
                throw new InvalidOperationException($"Employee '{request.Username}' does not have an associated Auth0 user ID.");
            }

            await _authHelper.UpdatePasswordAsync(employee.UserId, request.NewPassword);
        }

        /// <summary>
        /// Sends an Auth0 /oauth/token request with the provided payload.
        /// </summary>
        /// <param name="payload">Token request payload (grant type and parameters).</param>
        /// <returns>Auth0 token response.</returns>
        private async Task<AuthTokenResponseDto> RequestTokenAsync(object payload, string? errorMsg = "Authorization Failed.")
        {
            var domain = GetRequiredConfig("Authentication:Domain");
            var client = _httpClientFactory.CreateClient();
            var content = JsonContent.Create(payload);
            var request = new HttpRequestMessage(HttpMethod.Post, $"https://{domain}/oauth/token")
            {
                Content = content
            };

            var response = await client.SendAsync(request);
            
            if (!response.IsSuccessStatusCode)
            {
                await ThrowAuth0ExceptionAsync(response, errorMsg);
            }

            var token = await response.Content.ReadFromJsonAsync<AuthTokenResponseDto>();
            if (token == null || string.IsNullOrWhiteSpace(token.AccessToken))
            {
                throw new InvalidOperationException("Failed to read token response.");
            }

            return token;
        }

        /// <summary>
        /// Parses Auth0 error responses and throws a structured Auth0Exception.
        /// </summary>
        /// <param name="response">HTTP response returned by Auth0.</param>
        /// <param name="defaultMessage">Fallback message when details are unavailable.</param>
        /// <returns>Always throws; no return value.</returns>
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

        /// <summary>
        /// Reads a required configuration value or throws if missing.
        /// </summary>
        /// <param name="key">Configuration key to read.</param>
        /// <returns>Non-empty configuration value.</returns>
        private string GetRequiredConfig(string key)
        {
            var value = _configuration[key];
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException($"Missing configuration value: {key}.");
            }

            return value;
        }
    }
}
