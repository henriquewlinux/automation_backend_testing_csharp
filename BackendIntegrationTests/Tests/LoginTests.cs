using NUnit.Framework;
using BackendIntegrationTests.Routes;
using BackendIntegrationTests.Utils;
using BackendIntegrationTests.TestData;
using Newtonsoft.Json;
using System.Net;

namespace BackendIntegrationTests.Tests
{
    [TestFixture]
    public class LoginTests
    {
        private LoginRoute _loginRoute;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _loginRoute = new LoginRoute();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _loginRoute?.Dispose();
        }

        [Test]
        public async Task Login_WithValidCredentials_ShouldReturnSuccessAndValidToken()
        {
            // Act
            var response = await _loginRoute.LoginAsync(TestData.LoginCredentials.ValidEmail, TestData.LoginCredentials.ValidPassword);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                $"Expected status 200 but got {response.StatusCode}. Response: {response.Content}");

            Assert.That(response.Content, Is.Not.Null.And.Not.Empty,
                "Response content should not be null or empty");

            // Valida o schema da resposta
            var isValidSchema = SchemaValidator.ValidateJson(response.Content!, "LoginResponseSchema.json", out var errors);
            Assert.That(isValidSchema, Is.True,
                $"Response schema validation failed. Errors: {string.Join(", ", errors)}");

            // Extrai e valida o token
            var token = ExtractTokenFromResponse(response);
            Assert.That(token, Is.Not.Null.And.Not.Empty, "Token should be present in response");

            // Armazena o token para uso em outros testes
            TestData.AuthTokens.BearerToken = token;
        }

        [Test]
        public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
        {
            // Act
            var response = await _loginRoute.LoginAsync(TestData.LoginCredentials.InvalidEmail, TestData.LoginCredentials.InvalidPassword);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized),
                $"Expected status 401 but got {response.StatusCode}. Response: {response.Content}");

            Assert.That(response.Content, Is.Not.Null.And.Not.Empty,
                "Response content should not be null or empty");
        }

        [Test]
        public async Task Login_WithEmptyEmail_ShouldReturnBadRequest()
        {
            // Act
            var response = await _loginRoute.LoginAsync("", TestData.LoginCredentials.ValidPassword);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest),
                $"Expected status 400 but got {response.StatusCode}. Response: {response.Content}");
        }

        [Test]
        public async Task Login_WithEmptyPassword_ShouldReturnBadRequest()
        {
            // Act
            var response = await _loginRoute.LoginAsync(TestData.LoginCredentials.ValidEmail, "");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest),
                $"Expected status 400 but got {response.StatusCode}. Response: {response.Content}");
        }

        private string? ExtractTokenFromResponse(RestSharp.RestResponse response)
        {
            if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
            {
                try
                {
                    dynamic? jsonResponse = JsonConvert.DeserializeObject(response.Content);
                    return jsonResponse?.token?.ToString();
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }
    }
}