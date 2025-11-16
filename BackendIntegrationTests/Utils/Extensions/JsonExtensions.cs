using System.Text.Json;
using BackendIntegrationTests.Models.TestData;

namespace BackendIntegrationTests.Utils.Extensions;

public static class JsonExtensions
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public static string ToJson(this Credential credential)
    {
        return JsonSerializer.Serialize(credential, Options);
    }

    public static string ToJson(this Product product)
    {
        return JsonSerializer.Serialize(product, Options);
    }
}
