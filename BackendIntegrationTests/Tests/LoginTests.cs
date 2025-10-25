using NUnit.Framework;
using BackendIntegrationTests.Routes;
using BackendIntegrationTests.Utils;
using Newtonsoft.Json;
using System.Net;

namespace BackendIntegrationTests.Tests
{
    [TestFixture]
    public class LoginTests : IntegrationTestsSetup
    {
        readonly string validEmail = JsonDataReader.GetValue("credentials.valid.email");
        readonly string validPassword = JsonDataReader.GetValue("credentials.valid.password");
        readonly string invalidEmail = JsonDataReader.GetValue("credentials.invalid.email");
        readonly string invalidPassword = JsonDataReader.GetValue("credentials.invalid.password");

        [Test]
        public async Task Login_WithValidCredentials_ShouldReturnSuccessAndValidToken()
        {
            // Act
            var response = await LoginRoute.LoginAsync(validEmail, validPassword);

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
            var token = LoginRoute.ExtractTokenFromResponse(response);
            Assert.That(token, Is.Not.Null.And.Not.Empty, "Token should be present in response");
        }

        [Test]
        public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
        {
            // Act
            var response = await LoginRoute.LoginAsync(invalidEmail, invalidPassword);

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
            var response = await LoginRoute.LoginAsync("", validPassword);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest),
                $"Expected status 400 but got {response.StatusCode}. Response: {response.Content}");
        }

        [Test]
        public async Task Login_WithEmptyPassword_ShouldReturnBadRequest()
        {
            // Act
            var response = await LoginRoute.LoginAsync(validEmail, "");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest),
                $"Expected status 400 but got {response.StatusCode}. Response: {response.Content}");
        }
    }
}