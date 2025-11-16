using NUnit.Framework;
using BackendIntegrationTests.Routes;
using BackendIntegrationTests.Utils.Helpers;
using BackendIntegrationTests.Schemas;
using BackendIntegrationTests.Models.TestData;
using System.Net;
using Allure.NUnit.Attributes;
using Allure.Net.Commons;

namespace BackendIntegrationTests.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [AllureSuite("Authentication")]
    [AllureDisplayIgnored]
    public class LoginTests : IntegrationTestsSetup
    {
        private LoginRoute _loginRoute;

        // Test Data Properties - Access credentials from base class
        private static Credential ValidCredential => _credentials.Value["valid"];
        private static Credential InvalidCredential => _credentials.Value["invalid"];

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
        [AllureSeverity(SeverityLevel.critical)]
        [AllureDescription("Validates that a user can successfully login with valid credentials and receive a valid JWT token")]
        [AllureTag("login", "authentication", "positive")]
        [AllureOwner("QA Team")]
        public async Task Login_WithValidCredentials_ShouldReturnSuccessAndValidToken()
        {
            // Act
            var response = await _loginRoute.LoginAsync(ValidCredential.Email, ValidCredential.Password);

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
        [AllureSeverity(SeverityLevel.critical)]
        [AllureDescription("Validates that login fails with invalid credentials and returns 401 Unauthorized")]
        [AllureTag("login", "authentication", "negative", "security")]
        [AllureOwner("QA Team")]
        public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
        {
            // Act
            var response = await _loginRoute.LoginAsync(InvalidCredential.Email, InvalidCredential.Password);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized),
                $"Expected status 401 but got {response.StatusCode}. Response: {response.Content}");

            Assert.That(response.Content, Is.Not.Null.And.Not.Empty,
                "Response content should not be null or empty");
        }

        [Test]
        [AllureSeverity(SeverityLevel.normal)]
        [AllureDescription("Validates that login fails when email is empty and returns 400 Bad Request")]
        [AllureTag("login", "authentication", "validation", "negative")]
        [AllureOwner("QA Team")]
        public async Task Login_WithEmptyEmail_ShouldReturnBadRequest()
        {
            // Act
            var response = await _loginRoute.LoginAsync("", ValidCredential.Password);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest),
                $"Expected status 400 but got {response.StatusCode}. Response: {response.Content}");
        }

        [Test]
        [AllureSeverity(SeverityLevel.normal)]
        [AllureDescription("Validates that login fails when password is empty and returns 400 Bad Request")]
        [AllureTag("login", "authentication", "validation", "negative")]
        [AllureOwner("QA Team")]
        public async Task Login_WithEmptyPassword_ShouldReturnBadRequest()
        {
            // Act
            var response = await _loginRoute.LoginAsync(ValidCredential.Email, "");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest),
                $"Expected status 400 but got {response.StatusCode}. Response: {response.Content}");
        }
    }
}