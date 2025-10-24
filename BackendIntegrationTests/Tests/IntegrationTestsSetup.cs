using NUnit.Framework;
using BackendIntegrationTests.Routes;
using BackendIntegrationTests.Utils.Helpers;
using Newtonsoft.Json;

namespace BackendIntegrationTests.Tests
{
    [TestFixture]
    [Order(1)] // Executa primeiro para garantir que o token seja obtido
    public class IntegrationTestsSetup
    {
        private LoginRoute _loginRoute;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _loginRoute = new LoginRoute();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _loginRoute?.Dispose();
        }

        private string? ExtractTokenFromResponse(RestSharp.RestResponse response)
        {
            if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
            {
                try
                {
                    dynamic? jsonResponse = JsonConvert.DeserializeObject(response.Content);
                    return jsonResponse?.token?.ToString();
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }
    }
}