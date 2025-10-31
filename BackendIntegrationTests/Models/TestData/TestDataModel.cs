using Newtonsoft.Json;

namespace BackendIntegrationTests.Models.TestData;

public class TestDataModel
{
    public CredentialsData Credentials { get; set; } = new();
    public ProductsData Products { get; set; } = new();
}

public class CredentialsData
{
    public Credential Valid { get; set; } = new();
    public Credential Invalid { get; set; } = new();
}

public class ProductsData
{
    public Product Valid { get; set; } = new();
    public Product Invalid { get; set; } = new();
}

public static class TestDataExtensions
{
    public static string ToJson(this Product product)
    {
        return JsonConvert.SerializeObject(product);
    }

    public static string ToJson(this Credential credential)
    {
        return JsonConvert.SerializeObject(credential);
    }
}
