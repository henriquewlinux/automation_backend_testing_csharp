# Backend Integration Tests - C#

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
- **C# 12+** - Programming language
- **NUnit 4.0.1** - Testing framework
- **RestSharp 110.2.0** - HTTP client for API calls
- **Newtonsoft.Json 13.0.3** - JSON serialization/deserialization
- **Newtonsoft.Json.Schema 3.0.15** - JSON schema validation
- **Visual Studio Code** - Development environment

## Project Structure

```
automation_backend_testing_csharp/
├── BackendIntegrationTests/
│   ├── Data/                      # Test data
│   │   └── TestData.json          # Credentials and test data
│   ├── Routes/                    # API route handlers
│   │   ├── LoginRoute.cs          # Login endpoints
│   │   └── ProductsRoute.cs       # Products endpoints
│   ├── Schemas/                   # JSON schema validators
│   │   ├── LoginSchema.cs         # Login response schema
│   │   └── ProductSchema.cs       # Product response schema
│   ├── Tests/                     # Test cases
│   │   ├── LoginTests.cs          # Login test suite
│   │   └── ProductsTests.cs       # Products test suite
│   ├── Utils/                     # Utility classes
│   │   ├── Bases/
│   │   │   └── BaseName.cs        # Base configuration (URL, timeout)
│   │   └── Helpers/
│   │       ├── RestClientHelper.cs     # HTTP client wrapper
│   │       ├── HeadersBuilder.cs       # HTTP headers builder
│   │       └── SchemaValidator.cs      # Schema validation
│   ├── IntegrationTestsSetup.cs   # Base setup for all tests
│   └── BackendIntegrationTests.csproj
└── automation_backend_testing_csharp.sln
```

## Prerequisites

- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) or higher
- Application backend running at `http://localhost:3000`
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
```

### Via Visual Studio Code

The project has pre-configured tasks. Use:

- **F5** - Run all tests
- **Ctrl+Shift+P** → "Tasks: Run Task" → Choose:
  - `Run All Tests` - Execute all tests
  - `Run Login Tests` - Execute login tests only
  - `Run Products Tests` - Execute products tests only
  - `Build Project` - Compile the project

## Test Suites

### LoginTests

Validates the authentication endpoint (`/login`):

- ✅ Login with valid credentials (verifies JWT token and schema)
- ✅ Login with invalid credentials (should return 401)
- ✅ Login with empty email (should return 400)
- ✅ Login with empty password (should return 400)

### ProductsTests

Validates the product management endpoint (`/products`):

- ✅ List all products (validates array schema)
- ✅ Create product with valid data and token (should return 201)
- ✅ Create product without authentication (should return 401)
- ✅ Create product with invalid token (should return 401)
- ✅ Create product with invalid data (should return 400)
- ✅ Create product without name (should return 400)

## Configuration

### Base URL and Timeout

Edit the [BaseName.cs](BackendIntegrationTests/Utils/Bases/BaseName.cs) file to adjust:

```csharp
public const string BASEURL = "http://localhost:3000";
public const int TIMEOUTSECONDS = 30;
```

### Test Data

Test data is centralized in [TestData.json](BackendIntegrationTests/Data/TestData.json):

```json
{
  "credentials": {
    "valid": {
      "email": "teste@teste.com",
      "password": "teste"
    },
    "invalid": {
      "email": "invalid@email.com",
      "password": "wrongpassword"
    }
  },
  "products": {
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
}
```

## Schema Validation

The project uses JSON Schema to validate API responses:

- **LoginSchema**: Validates JWT token format, expiration, and user object
- **ProductSchema**: Validates UUID, name, price, stock, and timestamps
- **ProductsListSchema**: Validates product array

## Features

### RestClientHelper
RestSharp wrapper that provides:
- Automatic timeout configuration
- HTTP exception handling
- Support for GET, POST, PUT, DELETE methods

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
bool isValid = SchemaValidator.ValidateResponse(response, LoginSchema.GetSchema());
```

## Reports

Tests generate automatic console logs with:
- Executed test name
- Status (Passed/Failed)
- Detailed error messages

## Contributing

1. Fork the project
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is open source and available under the [MIT License](LICENSE).

## Contact

Henrique W. Linux - [@henriquewlinux](https://github.com/henriquewlinux)

Project Link: [https://github.com/henriquewlinux/automation_backend_testing_csharp](https://github.com/henriquewlinux/automation_backend_testing_csharp)
