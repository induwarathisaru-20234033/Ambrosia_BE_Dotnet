using System.Net;
using System.Text.Json;
using AMB.Application.Dtos;
using AMB.Application.Services;
using AMB.Domain.Entities;
using AMB.Domain.Enums;
using AMB.Tests.Mocks;
using Microsoft.Extensions.Configuration;

namespace AMB.Tests.AuthTests
{
    public class AuthServiceTests
    {
        [Fact]
        public async Task LoginAsync_WithActiveEmployee_ReturnsToken()
        {
            var handler = new TestHttpMessageHandler(_ =>
                new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"access_token\":\"token-123\",\"refresh_token\":\"refresh-123\",\"token_type\":\"Bearer\",\"expires_in\":3600}")
                });

            var httpClient = new HttpClient(handler);
            var httpClientFactory = new TestHttpClientFactory(httpClient);

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Authentication:Domain"] = "example.auth0.com",
                    ["Authentication:Audience"] = "https://example.auth0.com/api/v2/",
                    ["Authentication:ClientId"] = "client-id",
                    ["Authentication:ClientSecret"] = "client-secret",
                    ["Authentication:LogoutReturnTo"] = "https://app.example.com/logout"
                })
                .Build();

            var authHelper = new TestAuthHelper("auth-123");
            var employeeRepository = new TestEmployeeRepository
            {
                EmployeeByUsername = new Employee
                {
                    Username = "ana",
                    Status = (int)EntityStatus.Active,
                    UserId = "auth0|123"
                }
            };

            var service = new AuthService(config, httpClientFactory, authHelper, employeeRepository);

            var result = await service.LoginAsync(new AuthLoginRequestDto
            {
                Username = "ana",
                Password = "P@ssw0rd!"
            });

            Assert.Equal("token-123", result.AccessToken);
            Assert.Equal("refresh-123", result.RefreshToken);
            Assert.Equal("Bearer", result.TokenType);
            Assert.Equal(3600, result.ExpiresIn);

            Assert.NotNull(handler.LastRequest);
            Assert.Equal(HttpMethod.Post, handler.LastRequest!.Method);
            Assert.Equal("https://example.auth0.com/oauth/token", handler.LastRequest.RequestUri!.ToString());

            var content = await handler.LastRequest.Content!.ReadAsStringAsync();
            var json = JsonDocument.Parse(content).RootElement;
            Assert.Equal("password", json.GetProperty("grant_type").GetString());
            Assert.Equal("ana", json.GetProperty("username").GetString());
        }

        [Fact]
        public async Task LoginAsync_WithInactiveEmployee_ThrowsUnauthorizedAccessException()
        {
            var handler = new TestHttpMessageHandler(_ =>
                new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"access_token\":\"token-123\",\"token_type\":\"Bearer\",\"expires_in\":3600}")
                });

            var httpClient = new HttpClient(handler);
            var httpClientFactory = new TestHttpClientFactory(httpClient);

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Authentication:Domain"] = "example.auth0.com",
                    ["Authentication:Audience"] = "https://example.auth0.com/api/v2/",
                    ["Authentication:ClientId"] = "client-id",
                    ["Authentication:ClientSecret"] = "client-secret"
                })
                .Build();

            var authHelper = new TestAuthHelper("auth-123");
            var employeeRepository = new TestEmployeeRepository
            {
                EmployeeByUsername = new Employee
                {
                    Username = "ana",
                    Status = (int)EntityStatus.Inactive,
                    UserId = "auth0|123"
                }
            };

            var service = new AuthService(config, httpClientFactory, authHelper, employeeRepository);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.LoginAsync(new AuthLoginRequestDto
            {
                Username = "ana",
                Password = "P@ssw0rd!"
            }));
        }

        [Fact]
        public async Task BuildLogoutUrlAsync_WithReturnTo_UsesReturnTo()
        {
            var handler = new TestHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));
            var httpClient = new HttpClient(handler);
            var httpClientFactory = new TestHttpClientFactory(httpClient);

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Authentication:Domain"] = "example.auth0.com",
                    ["Authentication:ClientId"] = "client-id",
                    ["Authentication:LogoutReturnTo"] = "https://app.example.com/logout"
                })
                .Build();

            var authHelper = new TestAuthHelper("auth-123");
            var employeeRepository = new TestEmployeeRepository();
            var service = new AuthService(config, httpClientFactory, authHelper, employeeRepository);

            var result = await service.BuildLogoutUrlAsync(new LogoutRequestDto
            {
                ReturnTo = "https://app.example.com/bye"
            });

            Assert.Contains("https://example.auth0.com/v2/logout", result.LogoutUrl);
            Assert.Contains("client_id=client-id", result.LogoutUrl);
            Assert.Contains("returnTo=https%3A%2F%2Fapp.example.com%2Fbye", result.LogoutUrl);
        }
    }
}
