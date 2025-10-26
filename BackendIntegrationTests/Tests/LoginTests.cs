using NUnit.Framework;
using BackendIntegrationTests.Routes;
using BackendIntegrationTests.Utils.Helpers;
using BackendIntegrationTests.Schemas;
using System.Net;

namespace BackendIntegrationTests.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class LoginTests : IntegrationTestsSetup
    {
        private LoginRoute _loginRoute;

        [OneTimeSetUp]
        public void SetUpLoginRoute()
        {
            _loginRoute = new LoginRoute();
        }

        [OneTimeTearDown]
        public void TearDownLoginRoute()
        {
            _loginRoute?.Dispose();
        }

        [Test]
        public async Task Login_WithValidCredentials_ShouldReturnSuccessAndValidToken()
        {
            // Act
            var response = await _loginRoute.LoginAsync(GetDataValue("credentials.valid.email"), GetDataValue("credentials.valid.password"));

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                $"Expected status 200 but got {response.StatusCode}. Response: {response.Content}");

            Assert.That(response.Content, Is.Not.Null.And.Not.Empty,
                "Response content should not be null or empty");

            // Valida o schema da resposta
            SchemaValidator.AssertJsonSchema(response.Content, LoginSchema.Schema, "ShouldReturnSuccessAndValidToken");

            // Extrai e valida o token
            var token = _loginRoute.ExtractTokenFromResponse(response);
            Assert.That(token, Is.Not.Null.And.Not.Empty, "Token should be present in response");
        }

        [Test]
        public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
        {
            // Act
            var response = await _loginRoute.LoginAsync(GetDataValue("credentials.invalid.email"), GetDataValue("credentials.invalid.password"));

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
            var response = await _loginRoute.LoginAsync("", GetDataValue("credentials.valid.password"));

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest),
                $"Expected status 400 but got {response.StatusCode}. Response: {response.Content}");
        }

        [Test]
        public async Task Login_WithEmptyPassword_ShouldReturnBadRequest()
        {
            // Act
            var response = await _loginRoute.LoginAsync(GetDataValue("credentials.valid.email"), "");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest),
                $"Expected status 400 but got {response.StatusCode}. Response: {response.Content}");
        }
    }
}