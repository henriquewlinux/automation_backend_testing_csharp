using NUnit.Framework;
using BackendIntegrationTests.Routes;
using BackendIntegrationTests.Utils;
using Newtonsoft.Json;
using System.Net;

namespace BackendIntegrationTests.Tests
{
    [TestFixture]
    public class ProductsTests : IntegrationTestsSetup
    {
        private string? _token;
        
        [SetUp]
        public async Task SetUp()
        {
            _token = await LoginRoute.GetToken(JsonDataReader.GetValue("credentials.valid.email"), JsonDataReader.GetValue("credentials.valid.password"));
        }

        [Test]
        public async Task GetAllProducts_ShouldReturnSuccessAndValidSchema()
        {
            // Act
            var response = await ProductsRoute.GetAllProductsAsync();

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
            //var token = responseLogin.Content;
            Assert.That(_token, Is.Not.Null.And.Not.Empty,
                "Bearer token should be available for this test");

            // Act
            var validProduct = JsonDataReader.GetValue("products.valid");

            // Cria um produto com nome único para evitar duplicação
            var uniqueProduct = new
            {
                name = $"{validProduct.name} - {DateTime.Now:yyyyMMddHHmmss}",
                price = (int)validProduct.price,
                stock = (int)validProduct.stock
            };

            var response = await ProductsRoute.CreateProductAsync(uniqueProduct, _token);

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
            var response = await ProductsRoute.CreateProductAsync(JsonDataReader.GetValue("products.valid"));

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized),
                $"Expected status 401 but got {response.StatusCode}. Response: {response.Content}");
        }

        [Test]
        public async Task CreateProduct_WithInvalidToken_ShouldReturnUnauthorized()
        {
            // Act
            var response = await ProductsRoute.CreateProductAsync(JsonDataReader.GetValue("products.valid"), "invalid_token");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized),
                $"Expected status 401 but got {response.StatusCode}. Response: {response.Content}");
        }

        [Test]
        public async Task CreateProduct_WithInvalidData_ShouldReturnBadRequest()
        {
            // Arrange
            Assert.That(_token, Is.Not.Null.And.Not.Empty,
                "Bearer token should be available for this test");

            // Act
            var response = await ProductsRoute.CreateProductAsync(JsonDataReader.GetValue("products.invalid"), _token);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest),
                $"Expected status 400 but got {response.StatusCode}. Response: {response.Content}");
        }

        [Test]
        public async Task CreateProduct_WithMissingName_ShouldReturnBadRequest()
        {
            // Arrange
            Assert.That(_token, Is.Not.Null.And.Not.Empty,
                "Bearer token should be available for this test");

            var invalidProduct = new
            {
                price = 100,
                stock = 10
                // name is missing
            };

            // Act
            var response = await ProductsRoute.CreateProductAsync(invalidProduct, _token);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest),
                $"Expected status 400 but got {response.StatusCode}. Response: {response.Content}");
        }
    }
}