using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BackendIntegrationTests.Utils.Bases;
using NUnit.Framework;

[assembly: Parallelizable(ParallelScope.All)]
[assembly: LevelOfParallelism(8)]

namespace BackendIntegrationTests;

public class IntegrationTestsSetup
{
    public static string BaseUrl { get; set; } = BaseName.BASEURL;
    public static int TimeoutSeconds { get; set; } = BaseName.TIMEOUTSECONDS;
    private static readonly Lazy<JObject> _testData = new(() =>
    {
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Data", "TestData.json");
        return JObject.Parse(File.ReadAllText(Path.GetFullPath(path)));
    });

    // Return string
    public static string GetDataValue(string keyPath)
    {
        var keys = keyPath.Split('.');
        JToken? token = _testData.Value;
        
        foreach (var key in keys)
            token = token?[key];
            
        return token?.ToString() ?? throw new KeyNotFoundException($"Key '{keyPath}' not found");
    }

    // Return object dynamic
    public static dynamic GetDataObject(string keyPath)
    {
        var json = GetDataValue(keyPath);
        return JsonConvert.DeserializeObject(json) ??
               throw new InvalidOperationException($"Failed to deserialize '{keyPath}'");
    }

    [SetUp]
    public void SetUp()
    {
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