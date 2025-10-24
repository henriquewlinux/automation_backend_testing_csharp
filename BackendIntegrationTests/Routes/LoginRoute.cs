using RestSharp;
using BackendIntegrationTests.Utils;
using BackendIntegrationTests.Utils.Helpers;

namespace BackendIntegrationTests.Routes;
public class LoginRoute : RestClientHelper
{
    const string ROUTE = "/login";
    public async Task<RestResponse> LoginAsync(string email, string password)
    {
        var data = new
        {
            email = email,
            password = password
        };

        return await ExecuteAsyncPost(ROUTE, data);
    }
}