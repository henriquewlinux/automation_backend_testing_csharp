using NUnit.Framework;
using BackendIntegrationTests.Routes;
using BackendIntegrationTests.Schemas;
using System.Net;
using BackendIntegrationTests.Utils.Helpers;

namespace BackendIntegrationTests.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class ProductsTests : IntegrationTestsSetup
    {
        private ProductsRoute _productsRoute;
        private LoginRoute _loginRoute;
        private string? _token;

        [OneTimeSetUp]
        public async Task SetUpProductsRoute()
        {
            _productsRoute = new ProductsRoute();
            _loginRoute = new LoginRoute();
            _token = await _loginRoute.GetToken(GetDataValue("credentials.valid.email"), GetDataValue("credentials.valid.password"));
        }

        [OneTimeTearDown]
        public void TearDownProductsRoute()
        {
            _productsRoute?.Dispose();
            _loginRoute?.Dispose();
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

            // Assert Schema
            SchemaValidator.AssertJsonSchema(response.Content, ProductsListSchema.Schema, "ShouldReturnSuccessAndValidSchema");
        }

        [Test]
        public async Task CreateProduct_WithValidDataAndToken_ShouldReturnCreated()
        {
            // Arrange
            var validProduct = GetDataObject("products.valid");

            // Create new product
            var uniqueProduct = new
            {
                name = $"{validProduct.name} - {DateTime.Now:yyyyMMddHHmmss}",
                price = (int)validProduct.price,
                stock = (int)validProduct.stock
            };

            // Act
            var response = await _productsRoute.CreateProductAsync(uniqueProduct, _token);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created),
                $"Expected status 201 but got {response.StatusCode}. Response: {response.Content}");

            Assert.That(response.Content, Is.Not.Null.And.Not.Empty,
                "Response content should not be null or empty");

            // Assert Schema
            SchemaValidator.AssertJsonSchema(response.Content, ProductSchema.Schema, "ShouldReturnSuccessAndValidSchema");
        }

        [Test]
        public async Task CreateProduct_WithoutAuthentication_ShouldReturnUnauthorized()
        {
            // Act
            var response = await _productsRoute.CreateProductAsync(GetDataValue("products.valid"));

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized),
                $"Expected status 401 but got {response.StatusCode}. Response: {response.Content}");
        }

        [Test]
        public async Task CreateProduct_WithInvalidToken_ShouldReturnUnauthorized()
        {
            // Act
            var response = await _productsRoute.CreateProductAsync(GetDataValue("products.valid"), "invalid_token");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized),
                $"Expected status 401 but got {response.StatusCode}. Response: {response.Content}");
        }

        [Test]
        public async Task CreateProduct_WithInvalidData_ShouldReturnBadRequest()
        {
            // Act
            var response = await _productsRoute.CreateProductAsync(GetDataValue("products.invalid"), _token);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest),
                $"Expected status 400 but got {response.StatusCode}. Response: {response.Content}");
        }

        [Test]
        public async Task CreateProduct_WithMissingName_ShouldReturnBadRequest()
        {
            // Arrange
            var invalidProduct = new
            {
                price = 100,
                stock = 10
                // name is missing
            };

            // Act
            var response = await _productsRoute.CreateProductAsync(invalidProduct, _token);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest),
                $"Expected status 400 but got {response.StatusCode}. Response: {response.Content}");
        }
    }
}