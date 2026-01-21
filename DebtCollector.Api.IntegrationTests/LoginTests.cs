using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using System.Net;
using System.Threading.Tasks;
using DebtCollector.Application.Auth.Commands.Login;
using System.Net.Http.Json;

namespace DebtCollector.Api.IntegrationTests
{
    public class LoginTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public LoginTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var client = _factory.CreateClient();
            var command = new LoginCommand 
            { 
                Email = "invalid@example.com", 
                Password = "wrongpassword" 
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/auth/login", command);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
