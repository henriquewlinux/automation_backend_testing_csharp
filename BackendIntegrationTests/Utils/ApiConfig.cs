using BackendIntegrationTests.Utils.Bases;
using BackendIntegrationTests.Routes;

namespace BackendIntegrationTests.Utils
{
    public class ApiConfig
    {
        public static string BaseUrl { get; set; } = BaseName.BASEURL;
        public static int TimeoutSeconds { get; set; } = BaseName.TIMEOUTSECONDS;

        public class RouteManager : IDisposable
        {
            public ProductsRoute ProductsRoute { get; private set; }
            public LoginRoute LoginRoute { get; private set; }

            public RouteManager()
            {
                ProductsRoute = new ProductsRoute();
                LoginRoute = new LoginRoute();
            }

            public void Dispose()
            {
                ProductsRoute?.Dispose();
                LoginRoute?.Dispose();
            }
        }
    }
}