# Backend Integration Tests

Este projeto contém testes de integração automatizados para APIs backend, desenvolvido em C# com as seguintes tecnologias:

- **RestSharp**: Para realizar requisições HTTP
- **Newtonsoft.Json.Schema**: Para validação de schemas JSON
- **NUnit**: Framework de testes

## Estrutura do Projeto

```
BackendIntegrationTests/
├── Routes/                 # Classes para requisições HTTP
│   ├── LoginRoute.cs      # Requisições de login
│   └── ProductsRoute.cs   # Requisições de produtos
├── Schema/                # Schemas JSON para validação
│   ├── LoginResponseSchema.json
│   ├── ProductSchema.json
│   ├── ProductsListSchema.json
│   └── ErrorResponseSchema.json
├── Util/                  # Classes utilitárias e configuração
│   ├── ApiConfig.cs       # Configurações da API
│   ├── RestClientHelper.cs # Helper para RestClient
│   ├── SchemaValidator.cs # Validador de schemas
│   └── TestData.cs        # Dados de teste
└── Test/                  # Testes de integração
    ├── IntegrationTestsSetup.cs # Setup inicial dos testes
    ├── LoginTests.cs      # Testes de login
    └── ProductsTests.cs   # Testes de produtos
```

## Pré-requisitos

- .NET 8.0 ou superior
- API backend rodando em `http://localhost:3000`

## Configuração

### 1. Restaurar Dependências

```bash
cd BackendIntegrationTests
dotnet restore
```

### 2. Configurar a API Base URL

Se sua API estiver rodando em uma URL diferente, edite o arquivo `Util/ApiConfig.cs`:

```csharp
public static string BaseUrl { get; set; } = "http://localhost:3000";
```

### 3. Configurar Credenciais de Teste

Edite o arquivo `Util/TestData.cs` para configurar as credenciais de teste:

```csharp
public static class LoginCredentials
{
    public const string ValidEmail = "teste@teste.com";
    public const string ValidPassword = "teste";
    // ...
}
```

## Executando os Testes

### Executar Todos os Testes

```bash
dotnet test
```

### Executar Testes com Detalhes Verbosos

```bash
dotnet test --verbosity normal
```

### Executar Testes Específicos

```bash
# Apenas testes de login
dotnet test --filter "LoginTests"

# Apenas testes de produtos
dotnet test --filter "ProductsTests"

# Teste específico
dotnet test --filter "Login_WithValidCredentials_ShouldReturnSuccessAndValidToken"
```

### Executar com Relatório de Cobertura

```bash
dotnet test --collect:"XPlat Code Coverage"
```

## Funcionalidades Testadas

### Testes de Login (`LoginTests.cs`)

- ✅ Login com credenciais válidas
- ✅ Login com credenciais inválidas
- ✅ Login com email vazio
- ✅ Login com senha vazia
- ✅ Validação de schema da resposta
- ✅ Extração e validação do token JWT

### Testes de Produtos (`ProductsTests.cs`)

- ✅ Listar todos os produtos
- ✅ Criar produto com dados válidos e token
- ✅ Criar produto sem autenticação
- ✅ Criar produto com token inválido
- ✅ Criar produto com dados inválidos
- ✅ Criar produto com campos obrigatórios faltando
- ✅ Validação de schemas das respostas

## Schemas de Validação

### Login Response Schema
```json
{
  "type": "object",
  "properties": {
    "token": { "type": "string", "minLength": 1 },
    "user": {
      "type": "object",
      "properties": {
        "id": { "type": "string" },
        "email": { "type": "string", "format": "email" }
      },
      "required": ["id", "email"]
    }
  },
  "required": ["token"]
}
```

### Product Schema
```json
{
  "type": "object",
  "properties": {
    "id": { "type": "string" },
    "name": { "type": "string", "minLength": 1 },
    "price": { "type": "number", "minimum": 0 },
    "stock": { "type": "integer", "minimum": 0 },
    "createdAt": { "type": "string" },
    "updatedAt": { "type": "string" }
  },
  "required": ["id", "name", "price", "stock"]
}
```

## Configurações Avançadas

### Timeout das Requisições

Para alterar o timeout das requisições, edite `Util/ApiConfig.cs`:

```csharp
public static int TimeoutSeconds { get; set; } = 30;
```

### Headers Customizados

Para adicionar headers customizados, edite as classes em `Routes/`:

```csharp
request.AddHeader("Custom-Header", "value");
```

### Dados de Teste Customizados

Para usar dados de teste diferentes, edite `Util/TestData.cs`:

```csharp
public static object ValidProduct => new
{
    name = "Produto Personalizado",
    price = 999.99,
    stock = 50
};
```

## Troubleshooting

### Erro: "Connection refused"
- Verifique se a API está rodando em `http://localhost:3000`
- Confirme se não há firewall bloqueando a conexão

### Erro: "Schema validation failed"
- Verifique se a resposta da API está no formato esperado
- Compare a resposta real com o schema em `Schema/`

### Erro: "Token should be present in response"
- Verifique se as credenciais de login estão corretas
- Confirme se a API está retornando o token no formato esperado

### Testes Falhando por Timeout
- Aumente o valor de `TimeoutSeconds` em `ApiConfig.cs`
- Verifique se a API não está sobrecarregada

## Contribuindo

1. Adicione novos testes em `Test/`
2. Crie novos schemas em `Schema/` quando necessário
3. Adicione novas rotas em `Routes/` para novos endpoints
4. Mantenha os dados de teste organizados em `Util/TestData.cs`

## Exemplo de Execução

```bash
$ dotnet test

Iniciando execução de teste para BackendIntegrationTests.dll (.NETCoreApp,Version=v8.0)

Aprovado!  - Com falha:     0, Aprovado:    12, Ignorado:     0, Total:    12, Duração: 2s
```

## Logs e Debugging

Para habilitar logs detalhados, adicione ao seu teste:

```csharp
[Test]
public async Task MyTest()
{
    TestContext.WriteLine("Executando teste personalizado...");
    // seu código de teste
}
```

Os logs aparecerão na saída do teste quando executado com `--verbosity normal`.