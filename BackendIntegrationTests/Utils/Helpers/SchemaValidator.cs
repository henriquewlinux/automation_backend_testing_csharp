using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Linq;

namespace BackendIntegrationTests.Utils
{
    public class SchemaValidator
    {
        public static bool ValidateJson(string jsonContent, string schemaFileName, out IList<string> errors)
        {
            errors = new List<string>();

            try
            {
                var schemaPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Schemas", schemaFileName);

                if (!File.Exists(schemaPath))
                    {
                        throw new FileNotFoundException($"Schema file not found: {schemaPath}");
                    }

                string schemaContent = File.ReadAllText(schemaPath);
                var schema = JSchema.Parse(schemaContent);
                
                JToken jsonToken = JToken.Parse(jsonContent);
                return jsonToken.IsValid(schema, out errors);
            }
            catch (Exception ex)
            {
                errors.Add($"Validation error: {ex.Message}");
                return false;
            }
        }
    }
}