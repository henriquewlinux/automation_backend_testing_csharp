using NUnit.Framework;
using BackendIntegrationTests.Routes;
using BackendIntegrationTests.Utils;
using Newtonsoft.Json;
using System.Net;

namespace BackendIntegrationTests.Tests
{
    [TestFixture]
    public class ProductsTests
    {
        private ApiConfig.RouteManager _routeManager;
        private ProductsRoute _productsRoute;
        private LoginRoute _loginRoute;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _routeManager = new ApiConfig.RouteManager();
            _productsRoute = _routeManager.ProductsRoute;
            _loginRoute = _routeManager.LoginRoute;

            // Realiza login para obter token se ainda não tiver
            if (string.IsNullOrEmpty(TestData.AuthTokens.BearerToken))
            {
                var loginResponse = await _loginRoute.LoginAsync(TestData.LoginCredentials.ValidEmail, TestData.LoginCredentials.ValidPassword);
                if (loginResponse.IsSuccessful)
                {
                    TestData.AuthTokens.BearerToken = ExtractTokenFromResponse(loginResponse);
                }
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _routeManager?.Dispose();
        }

        [Test]
        public async Task GetAllProducts_ShouldReturnSuccessAndValidSchema()
        {
            // Act
            var response = await _productsRoute.GetAllProductsAsync();

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                $"Expected status 200 but got {response.StatusCode}. Response: {response.Content}");

            Assert.That(response.Content, Is.Not.Null,
                "Response content should not be null");

            // Valida o schema da resposta
            var isValidSchema = SchemaValidator.ValidateJson(response.Content!, "ProductsListSchema.json", out var errors);
            Assert.That(isValidSchema, Is.True,
                $"Response schema validation failed. Errors: {string.Join(", ", errors)}");
        }

        [Test]
        public async Task CreateProduct_WithValidDataAndToken_ShouldReturnCreated()
        {
            // Arrange
            //var responseLogin = await _loginRoute.LoginAsync(TestData.LoginCredentials.ValidEmail, TestData.LoginCredentials.ValidPassword);
            // JObject json = JObject.Parse(responseLogin.Content);
            // string token = json["chave"]?.ToString();
            //var token = responseLogin.Content;
            Assert.That(TestData.AuthTokens.BearerToken, Is.Not.Null.And.Not.Empty,
                "Bearer token should be available for this test");

            // Act
            var response = await _productsRoute.CreateProductAsync(TestData.ProductData.ValidProduct, TestData.AuthTokens.BearerToken);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created),
                $"Expected status 201 but got {response.StatusCode}. Response: {response.Content}");

            Assert.That(response.Content, Is.Not.Null.And.Not.Empty,
                "Response content should not be null or empty");

            // Valida o schema da resposta
            var isValidSchema = SchemaValidator.ValidateJson(response.Content!, "ProductSchema.json", out var errors);
            Assert.That(isValidSchema, Is.False,
                $"Response schema validation failed. Errors: {string.Join(", ", errors)}");
        }

        [Test]
        public async Task CreateProduct_WithoutAuthentication_ShouldReturnUnauthorized()
        {
            // Act
            var response = await _productsRoute.CreateProductAsync(TestData.ProductData.ValidProduct);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized),
                $"Expected status 401 but got {response.StatusCode}. Response: {response.Content}");
        }

        [Test]
        public async Task CreateProduct_WithInvalidToken_ShouldReturnUnauthorized()
        {
            // Act
            var response = await _productsRoute.CreateProductAsync(TestData.ProductData.ValidProduct, "invalid_token");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized),
                $"Expected status 401 but got {response.StatusCode}. Response: {response.Content}");
        }

        [Test]
        public async Task CreateProduct_WithInvalidData_ShouldReturnBadRequest()
        {
            // Arrange
            Assert.That(TestData.AuthTokens.BearerToken, Is.Not.Null.And.Not.Empty,
                "Bearer token should be available for this test");

            // Act
            var response = await _productsRoute.CreateProductAsync(TestData.ProductData.InvalidProduct, TestData.AuthTokens.BearerToken);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest),
                $"Expected status 400 but got {response.StatusCode}. Response: {response.Content}");
        }

        [Test]
        public async Task CreateProduct_WithMissingName_ShouldReturnBadRequest()
        {
            // Arrange
            Assert.That(TestData.AuthTokens.BearerToken, Is.Not.Null.And.Not.Empty,
                "Bearer token should be available for this test");

            var invalidProduct = new
            {
                price = 100,
                stock = 10
                // name is missing
            };

            // Act
            var response = await _productsRoute.CreateProductAsync(invalidProduct, TestData.AuthTokens.BearerToken);

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