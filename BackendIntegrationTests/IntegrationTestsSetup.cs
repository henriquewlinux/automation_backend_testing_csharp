using NUnit.Framework;
using BackendIntegrationTests.Routes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BackendIntegrationTests.Utils.Bases;

namespace BackendIntegrationTests;

[TestFixture]
[Order(1)] // Executa primeiro para garantir que o token seja obtido
public class IntegrationTestsSetup
{
    public static string BaseUrl { get; set; } = BaseName.BASEURL;
    public static int TimeoutSeconds { get; set; } = BaseName.TIMEOUTSECONDS;
    private LoginRoute _loginRoute;

    [OneTimeSetUp]
    public async Task OneTimeSetUpAsync()
    {
        _loginRoute = new LoginRoute();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _loginRoute?.Dispose();
    }

    public static void GlobalSetUp()
    {
        // Configurações globais que serão executadas antes de todos os testes
        TestContext.WriteLine("=== Iniciando Testes de Integração Backend ===");
        TestContext.WriteLine($"API Base URL: {BaseUrl}");
        TestContext.WriteLine($"Timeout: {TimeoutSeconds} segundos");
        TestContext.WriteLine("===============================================");
    }

    public static void GlobalTearDown()
    {
        // Limpeza global após todos os testes
        TestContext.WriteLine("=== Finalizando Testes de Integração Backend ===");
        TestContext.WriteLine("Todos os testes foram executados.");
        TestContext.WriteLine("================================================");
    }
}

/// <summary>
/// Classe para ler valores do arquivo TestData.json usando notação de ponto
/// </summary>
public static class JsonDataReader
{
    private static JObject? _testData;
    private static readonly object _lock = new object();

    /// <summary>
    /// Obtém um valor do TestData.json usando notação de ponto
    /// </summary>
    /// <param name="keyPath">Caminho da chave usando notação de ponto (ex: "credentials.valid" ou "credentials.valid.email")</param>
    /// <returns>Objeto dinâmico que pode ser um objeto completo ou valor específico. Para objetos, use .propriedade para acessar campos.</returns>
    /// <exception cref="InvalidOperationException">Lançada quando o arquivo não pode ser carregado</exception>
    /// <exception cref="KeyNotFoundException">Lançada quando a chave não é encontrada no JSON</exception>
    /// <example>
    /// // Obter objeto completo
    /// dynamic validCredentials = JsonDataReader.GetValue("credentials.valid");
    /// string email = validCredentials.email;
    /// string password = validCredentials.password;
    ///
    /// // Obter valor específico
    /// string email = JsonDataReader.GetValue("credentials.valid.email");
    /// </example>
    public static dynamic GetValue(string keyPath)
    {
        if (string.IsNullOrWhiteSpace(keyPath))
        {
            throw new ArgumentException("Key path cannot be null or empty", nameof(keyPath));
        }

        EnsureTestDataLoaded();

        // Divide o caminho por ponto e navega pelo JSON
        var keys = keyPath.Split('.');
        JToken? currentToken = _testData;

        for (int i = 0; i < keys.Length; i++)
        {
            if (currentToken == null)
            {
                var failedKey = i > 0 ? keys[i - 1] : "root";
                throw new KeyNotFoundException($"Key path '{keyPath}' not found in TestData.json. Failed at segment: '{failedKey}'");
            }

            currentToken = currentToken[keys[i]];

            if (currentToken == null)
            {
                throw new KeyNotFoundException($"Key path '{keyPath}' not found in TestData.json. Missing key: '{keys[i]}'");
            }
        }

        // Se for um objeto ou array, retorna como objeto dinâmico limpo (sem metadados do JObject)
        if (currentToken is JObject || currentToken is JArray)
        {
            // Serializa e deserializa para obter um objeto limpo sem metadados do Newtonsoft.Json
            var jsonString = currentToken.ToString(Formatting.None);
            return JsonConvert.DeserializeObject<dynamic>(jsonString)
                ?? throw new InvalidOperationException($"Failed to deserialize object at '{keyPath}'");
        }

        // Para valores primitivos (string, int, bool, etc), retorna o valor convertido diretamente
        var value = currentToken!.ToObject<dynamic>();
        return value ?? throw new InvalidOperationException($"Failed to convert value at '{keyPath}'");
    }

    private static void EnsureTestDataLoaded()
    {
        if (_testData == null)
        {
            lock (_lock)
            {
                if (_testData == null)
                {
                    LoadTestData();
                }
            }
        }
    }

    private static void LoadTestData()
    {
        try
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var projectRoot = Path.Combine(baseDirectory, "..", "..", "..");
            var testDataPath = Path.Combine(projectRoot, "Data", "TestData.json");
            var fullPath = Path.GetFullPath(testDataPath);

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"TestData.json file not found at: {fullPath}");
            }

            var jsonContent = File.ReadAllText(fullPath);
            _testData = JObject.Parse(jsonContent);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to load TestData.json: {ex.Message}", ex);
        }
    }
}