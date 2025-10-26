using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace BackendIntegrationTests.Utils
{
    public class SchemaValidator
    {
        public static bool ValidateJson<T>(string jsonContent, out IList<string> errors)
        {
            errors = new List<string>();

            try
            {
                // Use reflection to get the 'Schema' property from the type T
                var schemaProperty = typeof(T).GetProperty("Schema", BindingFlags.Public | BindingFlags.Static);

                if (schemaProperty == null)
                {
                    errors.Add($"Schema property not found in type {typeof(T).Name}. Make sure the class has a public static 'Schema' property.");
                    return false;
                }

                // Get the schema string value
                var schemaString = schemaProperty.GetValue(null) as string;

                if (string.IsNullOrEmpty(schemaString))
                {
                    errors.Add($"Schema property in type {typeof(T).Name} is null or empty.");
                    return false;
                }

                // Parse the JSON schema
                var schema = JSchema.Parse(schemaString);

                // Parse the JSON content
                JToken jsonToken = JToken.Parse(jsonContent);

                // Validate the JSON against the schema
                return jsonToken.IsValid(schema, out errors);
            }
            catch (JSchemaException ex)
            {
                errors.Add($"Schema parsing error: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                errors.Add($"Validation error: {ex.Message}");
                return false;
            }
        }
    }
}