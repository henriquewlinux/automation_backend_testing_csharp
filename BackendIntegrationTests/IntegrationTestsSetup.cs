using System.Text.Json;
using BackendIntegrationTests.Utils.Bases;
using BackendIntegrationTests.Models.TestData;
using NUnit.Framework;
using Allure.Net.Commons;
using Allure.NUnit;

[assembly: Parallelizable(ParallelScope.All)]
[assembly: LevelOfParallelism(8)]

namespace BackendIntegrationTests;

[AllureNUnit]
public class IntegrationTestsSetup
{
    public static string BaseUrl { get; set; } = BaseName.BASEURL;
    public static int TimeoutSeconds { get; set; } = BaseName.TIMEOUTSECONDS;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    // Load credentials from JSON - Protected for access by test classes
    protected static readonly Lazy<Dictionary<string, Credential>> _credentials = new(() =>
    {
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Data", "credentials.json");
        var json = File.ReadAllText(Path.GetFullPath(path));
        return JsonSerializer.Deserialize<Dictionary<string, Credential>>(json, JsonOptions)
            ?? throw new InvalidOperationException("Failed to load credentials data");
    });

    // Load products from JSON - Protected for access by test classes
    protected static readonly Lazy<Dictionary<string, Product>> _products = new(() =>
    {
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Data", "products.json");
        var json = File.ReadAllText(Path.GetFullPath(path));
        return JsonSerializer.Deserialize<Dictionary<string, Product>>(json, JsonOptions)
            ?? throw new InvalidOperationException("Failed to load products data");
    });

    [SetUp]
    public void SetUp()
    {
        TestContext.WriteLine("=============================================================");
        TestContext.WriteLine($"Starting test: {TestContext.CurrentContext.Test.Name}");
    }

    [TearDown]
    public void TearDown()
    {
        var testStatus = TestContext.CurrentContext.Result.Outcome.Status;
        var status = testStatus == NUnit.Framework.Interfaces.TestStatus.Passed ? "success" : "fail";
        TestContext.WriteLine($"Test Finished: {TestContext.CurrentContext.Test.Name} - Status: {status}");
        TestContext.WriteLine("=============================================================");
    }
}