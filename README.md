# Backend Integration Tests - C#

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![NUnit](https://img.shields.io/badge/NUnit-4.0.1-green?logo=nunit)
![Allure](https://img.shields.io/badge/Allure-2.14.1-orange)
![License](https://img.shields.io/badge/license-MIT-blue.svg)

Automated integration testing project for REST API validation using C# and .NET 8.0.

## About the Project

This project is a complete automated test suite for validating REST API endpoints, including authentication (login) and product management functionalities. The framework verifies:

- HTTP response status codes
- JSON schema validation
- JWT authentication and authorization
- Success and error scenarios
- Edge cases and invalid inputs

## Technologies Used

- **.NET 8.0** - Main framework
- **C# 12+** - Programming language with implicit usings
- **NUnit 4.0.1** - Testing framework
- **RestSharp 112.1.0** - HTTP client for API calls
- **Newtonsoft.Json 13.0.4** - JSON serialization/deserialization
- **Newtonsoft.Json.Schema 3.0.15** - JSON schema validation
- **Allure.NUnit 2.14.1** - Advanced test reporting and analytics
- **System.Text.Json** - Modern JSON handling
- **Visual Studio Code** - Development environment

## Project Structure

```
automation_backend_testing_csharp/
├── BackendIntegrationTests/
│   ├── Data/                           # Test data (JSON files)
│   │   ├── credentials.json            # Valid/invalid login credentials
│   │   └── products.json               # Valid/invalid product data
│   ├── Models/                         # Data models
│   │   ├── Credential.cs               # Email/Password model
│   │   └── Product.cs                  # Product model with unique name generator
│   ├── Routes/                         # API route handlers
│   │   ├── LoginRoute.cs               # POST /login endpoint wrapper
│   │   └── ProductsRoute.cs            # GET/POST /products endpoint wrapper
│   ├── Schemas/                        # JSON schema validators
│   │   ├── LoginSchema.cs              # Login response schema
│   │   └── ProductSchema.cs            # Product response schemas
│   ├── Tests/                          # Test suites
│   │   ├── LoginTests.cs               # 4 authentication tests
│   │   └── ProductsTests.cs            # 6 product management tests
│   ├── Utils/                          # Utility classes
│   │   ├── Bases/
│   │   │   └── BaseName.cs             # Base configuration (URL, timeout)
│   │   ├── Extensions/
│   │   │   └── JsonExtensions.cs       # ToJson() extension methods
│   │   └── Helpers/
│   │       ├── RestClientHelper.cs     # HTTP client wrapper
│   │       ├── HeadersBuilder.cs       # HTTP headers builder
│   │       └── SchemaValidator.cs      # Schema validation helper
│   ├── IntegrationTestsSetup.cs        # Base setup for all tests
│   ├── allureConfig.json               # Allure reporting configuration
│   └── BackendIntegrationTests.csproj  # Project file
├── .vscode/                            # VS Code configuration
│   ├── tasks.json                      # Build and test tasks
│   └── launch.json                     # Debug configurations
└── automation_backend_testing_csharp.sln
```

## Architecture

### Design Patterns

- **Repository Pattern**: `RestClientHelper` acts as a base HTTP client repository
- **Builder Pattern**: `HeadersBuilder` provides fluent API for building headers
- **Page Object Model** (adapted for APIs): Each `Route` class represents an API endpoint
- **Lazy Loading**: Test data is loaded on-demand for performance
- **Extension Methods**: `JsonExtensions` provides convenient serialization

### Test Data Management

Test data is separated into dedicated JSON files and loaded lazily:

- **credentials.json**: Contains valid and invalid login credentials
- **products.json**: Contains valid and invalid product data

Each test class accesses data through protected static properties inherited from `IntegrationTestsSetup`:

```csharp
private static Credential ValidCredential => _credentials.Value["valid"];
private static Product ValidProduct => _products.Value["valid"];
```

### Parallel Execution

Tests run in parallel with **8 concurrent threads** for optimal performance:

- All tests are parallelizable (`[Parallelizable(ParallelScope.All)]`)
- Unique product names generated using timestamps to prevent conflicts
- Thread-safe data loading with `Lazy<T>`

## Prerequisites

- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) or higher
- **Backend API running at `http://localhost:3000`** (required for tests to pass)
- Visual Studio Code (recommended) or Visual Studio

## Installation

1. Clone the repository:
```bash
git clone https://github.com/henriquewlinux/automation_backend_testing_csharp.git
cd automation_backend_testing_csharp
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Build the project:
```bash
dotnet build
```

## Running the Tests

### Via Command Line

Run all tests:
```bash
dotnet test
```

Run specific tests:
```bash
# Login tests only
dotnet test --filter "FullyQualifiedName~LoginTests"

# Products tests only
dotnet test --filter "FullyQualifiedName~ProductsTests"

# Tests with verbose output
dotnet test --verbosity normal
```

### Via Visual Studio Code

The project has pre-configured tasks. Use:

- **F5** - Run all tests
- **Ctrl+Shift+P** → "Tasks: Run Task" → Choose:
  - `Run All Tests` - Execute all tests
  - `Run Login Tests` - Execute login tests only
  - `Run Products Tests` - Execute products tests only
  - `Build Project` - Compile the project

## API Endpoints Tested

| Method | Endpoint | Authentication | Description |
|--------|----------|---------------|-------------|
| POST | `/login` | No | Authenticate user and receive JWT token |
| GET | `/products` | Optional | List all products |
| POST | `/products` | Required | Create a new product (requires Bearer token) |

### API Request/Response Examples

**POST /login**
```json
// Request
{
  "email": "teste@teste.com",
  "password": "teste"
}

// Response (200 OK)
{
  "token": "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2025-01-15T12:00:00Z",
  "user": {
    "name": "John",
    "surname": "Doe"
  },
  "message": "Login successful",
  "success": true
}
```

**POST /products**
```json
// Request (with Authorization: Bearer <token>)
{
  "name": "Samsung A80",
  "price": 1233,
  "stock": 15
}

// Response (201 Created)
{
  "success": true,
  "message": "Product created successfully",
  "data": {
    "uuid": "550e8400-e29b-41d4-a716-446655440000",
    "name": "Samsung A80",
    "price": 1233,
    "stock": 15,
    "createdAt": "2025-01-15T10:30:00Z",
    "updatedAt": "2025-01-15T10:30:00Z"
  }
}
```

## Test Suites

### LoginTests (Authentication Suite)

Validates the authentication endpoint (`/login`):

| Test | Severity | Status Codes | Tags |
|------|----------|--------------|------|
| Login with valid credentials | Critical | 200 OK | login, authentication, positive |
| Login with invalid credentials | Critical | 401 Unauthorized | login, authentication, negative, security |
| Login with empty email | Normal | 400 Bad Request | login, authentication, validation, negative |
| Login with empty password | Normal | 400 Bad Request | login, authentication, validation, negative |

### ProductsTests (Product Management Suite)

Validates the product management endpoints (`/products`):

| Test | Severity | Status Codes | Tags |
|------|----------|--------------|------|
| List all products | Critical | 200 OK | products, get, positive, schema |
| Create product with valid data and token | Critical | 201 Created | products, create, post, positive, authentication |
| Create product without authentication | Critical | 401 Unauthorized | products, create, post, negative, authentication, security |
| Create product with invalid token | Critical | 401 Unauthorized | products, create, post, negative, authentication, security |
| Create product with invalid data | Normal | 400 Bad Request | products, create, post, negative, validation |
| Create product without name | Normal | 400 Bad Request | products, create, post, negative, validation |

**Total Tests: 10** (7 Critical, 3 Normal)

## Configuration

### Base URL and Timeout

Edit the [BaseName.cs](BackendIntegrationTests/Utils/Bases/BaseName.cs) file to adjust:

```csharp
public const string BASEURL = "http://localhost:3000";
public const int TIMEOUTSECONDS = 30;
```

### Test Data

Test credentials are in [credentials.json](BackendIntegrationTests/Data/credentials.json):

```json
{
  "valid": {
    "email": "teste@teste.com",
    "password": "teste"
  },
  "invalid": {
    "email": "invalid@email.com",
    "password": "wrongpassword"
  }
}
```

Test products are in [products.json](BackendIntegrationTests/Data/products.json):

```json
{
  "valid": {
    "name": "Samsung A80",
    "price": 1233,
    "stock": 15
  },
  "invalid": {
    "name": "",
    "price": -1,
    "stock": -5
  }
}
```

### Allure Configuration

The Allure report configuration is in [allureConfig.json](BackendIntegrationTests/allureConfig.json):

```json
{
  "allure": {
    "directory": "allure-results",
    "title": "Report Automation Backend Testing",
    "links": [
      "https://github.com/henriquewlinux/automation_backend_testing_csharp/issues/{issue}",
      "https://github.com/henriquewlinux/automation_backend_testing_csharp/issues/{tms}"
    ]
  }
}
```

> **Note**: The title can be customized to match your project name.

## Schema Validation

The project uses JSON Schema to validate API responses:

- **LoginSchema**: Validates JWT token format, expiration, user object, and success message
- **ProductSchema**: Validates UUID v4, name, price (>= 0), stock (>= 0), and ISO8601 timestamps
- **ProductsListSchema**: Validates product array structure

## Features

### RestClientHelper
RestSharp wrapper that provides:
- Automatic timeout configuration (30 seconds default)
- HTTP exception handling
- Support for GET, POST, PUT, DELETE methods
- Overloaded methods for different payload types

### HeadersBuilder
Fluent HTTP headers builder:
```csharp
var headers = new HeadersBuilder()
    .AddContentType("application/json")
    .AddAuthorization($"Bearer {token}")
    .Build();
```

### SchemaValidator
JSON response validation against defined schemas:
```csharp
SchemaValidator.AssertJsonSchema(response, LoginSchema.Schema, testName);
```

### JsonExtensions
Convenient extension methods for JSON serialization:
```csharp
var json = product.ToJson();  // Serializes to camelCase JSON
```

## Reports

### Console Reports

Tests generate automatic console logs with:
- Test start/end markers
- Executed test name
- Status (success/fail)
- Detailed error messages

Example output:
```
=============================================================
Starting test: Login_WithValidCredentials_ShouldReturnSuccessAndValidToken
Test Finished: Login_WithValidCredentials_ShouldReturnSuccessAndValidToken - Status: success
=============================================================
```

### Allure Reports

This project is integrated with **Allure Framework** for comprehensive test reporting with rich visualizations, historical trends, and detailed test analytics.

#### Installing Allure CLI

**Windows (via Scoop):**
```bash
scoop install allure
```

**macOS:**
```bash
brew install allure
```

**Linux:**
```bash
# Download and extract
wget https://github.com/allure-framework/allure2/releases/download/2.24.0/allure-2.24.0.tgz
tar -zxvf allure-2.24.0.tgz -C /opt/
sudo ln -s /opt/allure-2.24.0/bin/allure /usr/bin/allure
```

**Manual Installation:**
Download from [Allure releases](https://github.com/allure-framework/allure2/releases) and add to PATH.

#### Generating Allure Reports

1. **Run tests** (this will generate results in `allure-results` directory):
```bash
dotnet test
```

2. **Generate and open the report**:
```bash
# Generate HTML report and open in browser
allure serve BackendIntegrationTests/bin/Debug/net8.0/allure-results
```

Or generate static report:
```bash
# Generate report to allure-report directory
allure generate BackendIntegrationTests/bin/Debug/net8.0/allure-results -o allure-report --clean

# Open the report
allure open allure-report
```

#### Allure Report Features

The Allure reports provide:
- **Overview Dashboard**: Summary statistics, success rate, test duration trends
- **Suites**: Tests organized by suites (Authentication, Product Management)
- **Graphs**: Visual charts showing test distribution by status, severity, and duration
- **Timeline**: Execution timeline showing parallel test execution (8 threads)
- **Behaviors**: Tests grouped by feature/behavior
- **Categories**: Failed tests categorized by error type
- **Test Details**: Each test includes:
  - Description and severity level (Critical/Normal)
  - Steps and assertions
  - Test output logs
  - Error messages and stack traces
  - Tags for filtering (authentication, products, security, validation, etc.)
  - Execution history and retry information
  - Owner: QA Team

#### Understanding Test Metadata

Tests are enriched with Allure attributes:
- **Severity Levels**:
  - Critical (7 tests): Core functionality like authentication and product creation
  - Normal (3 tests): Validation edge cases
- **Tags**: Filterable labels like `login`, `products`, `authentication`, `validation`, `security`, `positive`, `negative`
- **Descriptions**: Detailed test purpose and validation criteria
- **Owner**: QA Team (all tests)

## Troubleshooting

### Backend API Not Running

**Problem**: Tests fail with connection errors

**Solution**: Ensure the backend API is running at `http://localhost:3000` before executing tests.

```bash
# Check if API is accessible
curl http://localhost:3000/products

# Expected: Response with product list or empty array
```

### Test Data Issues

**Problem**: Tests fail due to invalid credentials

**Solution**: Verify the credentials in `credentials.json` match your backend's test user:

```json
{
  "valid": {
    "email": "teste@teste.com",  // Must match backend test user
    "password": "teste"
  }
}
```

### Allure Report Not Generating

**Problem**: `allure: command not found`

**Solution**: Install Allure CLI (see installation section above) and ensure it's in your PATH.

**Problem**: Empty Allure report

**Solution**: Run tests first to generate `allure-results` directory before generating the report.

### Parallel Test Conflicts

**Problem**: Product creation tests interfere with each other

**Solution**: Tests already use unique product names with timestamps. If conflicts persist, reduce parallelism:

```csharp
[assembly: LevelOfParallelism(4)]  // Reduce from 8 to 4 threads
```

### Schema Validation Failures

**Problem**: Tests fail with schema validation errors

**Solution**: The backend API response format may have changed. Update the corresponding schema in `Schemas/` directory to match the actual API response.

## Performance

- **Average test execution time**: ~500ms for all 10 tests
- **Parallel threads**: 8 concurrent test executions
- **Typical full suite duration**: < 1 second

## Contributing

1. Fork the project
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

### Code Style Guidelines

- Follow C# naming conventions (PascalCase for classes/methods, camelCase for variables)
- Add XML documentation comments for public methods
- Use descriptive test names following pattern: `MethodName_Scenario_ExpectedBehavior`
- Include Allure attributes: `[AllureDescription]`, `[AllureSeverity]`, `[AllureTag]`

## License

This project is open source and available under the [MIT License](LICENSE).

## Contact

Henrique W. Linux - [@henriquewlinux](https://github.com/henriquewlinux)

Project Link: [https://github.com/henriquewlinux/automation_backend_testing_csharp](https://github.com/henriquewlinux/automation_backend_testing_csharp)

---

**Built with** ❤️ **using .NET 8.0, NUnit, and Allure Framework**
