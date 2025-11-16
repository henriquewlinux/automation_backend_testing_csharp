using NUnit.Framework;
using BackendIntegrationTests.Routes;
using BackendIntegrationTests.Schemas;
using System.Net;
using BackendIntegrationTests.Utils.Helpers;
using BackendIntegrationTests.Models.TestData;
using BackendIntegrationTests.Utils.Extensions;
using Allure.NUnit.Attributes;
using Allure.Net.Commons;

namespace BackendIntegrationTests.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [AllureSuite("Product Management")]
    [AllureDisplayIgnored]
    public class ProductsTests : IntegrationTestsSetup
    {
        private ProductsRoute _productsRoute;
        private LoginRoute _loginRoute;
        private string? _token;

        // Test Data Properties - Access data from base class
        private static Credential ValidCredential => _credentials.Value["valid"];
        private static Product ValidProduct => _products.Value["valid"];
        private static Product InvalidProduct => _products.Value["invalid"];

        [OneTimeSetUp]
        public async Task SetUpProductsRoute()
        {
            _productsRoute = new ProductsRoute();
            _loginRoute = new LoginRoute();
            _token = await _loginRoute.GetToken(ValidCredential.Email, ValidCredential.Password);
        }

        [OneTimeTearDown]
        public void TearDownProductsRoute()
        {
            _productsRoute?.Dispose();
            _loginRoute?.Dispose();
        }

        [Test]
        [AllureSeverity(SeverityLevel.critical)]
        [AllureDescription("Validates that GET /products returns all products with valid schema and 200 OK status")]
        [AllureTag("products", "get", "positive", "schema")]
        [AllureOwner("QA Team")]
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
        [AllureSeverity(SeverityLevel.critical)]
        [AllureDescription("Validates that a product can be created with valid data and authentication token, returning 201 Created")]
        [AllureTag("products", "create", "post", "positive", "authentication")]
        [AllureOwner("QA Team")]
        public async Task CreateProduct_WithValidDataAndToken_ShouldReturnCreated()
        {
            // Arrange - Create product with unique name
            var uniqueProduct = ValidProduct.WithUniqueName();

            // Act
            var response = await _productsRoute.CreateProductAsync(uniqueProduct, _token!);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created),
                $"Expected status 201 but got {response.StatusCode}. Response: {response.Content}");

            Assert.That(response.Content, Is.Not.Null.And.Not.Empty,
                "Response content should not be null or empty");

            // Assert Schema
            SchemaValidator.AssertJsonSchema(response.Content, ProductSchema.Schema, "ShouldReturnSuccessAndValidSchema");
        }

        [Test]
        [AllureSeverity(SeverityLevel.critical)]
        [AllureDescription("Validates that creating a product without authentication token returns 401 Unauthorized")]
        [AllureTag("products", "create", "post", "negative", "authentication", "security")]
        [AllureOwner("QA Team")]
        public async Task CreateProduct_WithoutAuthentication_ShouldReturnUnauthorized()
        {
            // Act
            var response = await _productsRoute.CreateProductAsync(ValidProduct.ToJson());

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized),
                $"Expected status 401 but got {response.StatusCode}. Response: {response.Content}");
        }

        [Test]
        [AllureSeverity(SeverityLevel.critical)]
        [AllureDescription("Validates that creating a product with an invalid token returns 401 Unauthorized")]
        [AllureTag("products", "create", "post", "negative", "authentication", "security")]
        [AllureOwner("QA Team")]
        public async Task CreateProduct_WithInvalidToken_ShouldReturnUnauthorized()
        {
            // Act
            var response = await _productsRoute.CreateProductAsync(ValidProduct.ToJson(), "invalid_token");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized),
                $"Expected status 401 but got {response.StatusCode}. Response: {response.Content}");
        }

        [Test]
        [AllureSeverity(SeverityLevel.normal)]
        [AllureDescription("Validates that creating a product with invalid data (negative price/stock) returns 400 Bad Request")]
        [AllureTag("products", "create", "post", "negative", "validation")]
        [AllureOwner("QA Team")]
        public async Task CreateProduct_WithInvalidData_ShouldReturnBadRequest()
        {
            // Act
            var response = await _productsRoute.CreateProductAsync(InvalidProduct.ToJson(), _token!);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest),
                $"Expected status 400 but got {response.StatusCode}. Response: {response.Content}");
        }

        [Test]
        [AllureSeverity(SeverityLevel.normal)]
        [AllureDescription("Validates that creating a product without required 'name' field returns 400 Bad Request")]
        [AllureTag("products", "create", "post", "negative", "validation")]
        [AllureOwner("QA Team")]
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
            var response = await _productsRoute.CreateProductAsync(invalidProduct, _token!);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest),
                $"Expected status 400 but got {response.StatusCode}. Response: {response.Content}");
        }
    }
}