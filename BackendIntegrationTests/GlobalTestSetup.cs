using NUnit.Framework;

[assembly: LevelOfParallelism(1)]
[assembly: Parallelizable(ParallelScope.None)]

namespace BackendIntegrationTests
{
    [SetUpFixture]
    public class GlobalTestSetup
    {
        [OneTimeSetUp]
        public void GlobalSetUp()
        {
            // Configurações globais que serão executadas antes de todos os testes
            TestContext.WriteLine("=== Iniciando Testes de Integração Backend ===");
            TestContext.WriteLine($"API Base URL: {Utils.ApiConfig.BaseUrl}");
            TestContext.WriteLine($"Timeout: {Utils.ApiConfig.TimeoutSeconds} segundos");
            TestContext.WriteLine("===============================================");
        }

        [OneTimeTearDown]
        public void GlobalTearDown()
        {
            // Limpeza global após todos os testes
            TestContext.WriteLine("=== Finalizando Testes de Integração Backend ===");
            TestContext.WriteLine("Todos os testes foram executados.");
            TestContext.WriteLine("================================================");
        }
    }
}