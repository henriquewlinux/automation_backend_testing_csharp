using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace BackendIntegrationTests.Utils.Helpers;
public class SchemaValidator
{
    public static void AssertJsonSchema(string jsonResponse, string schema, string testName = "")
    {
        var schemaObj = JSchema.Parse(schema);
        var json = JToken.Parse(jsonResponse);

        IList<string> errors;
        bool isValid = json.IsValid(schemaObj, out errors);

        var errorMessage = string.IsNullOrEmpty(testName)
            ? $"Schema validation failed"
            : $"Schema validation failed for {testName}";

        if (!isValid)
        {
            errorMessage += $"\nErrors: {string.Join(", ", errors)}";
        }

        Assert.That(isValid, Is.True, errorMessage);
    }
}