using RestSharp;
using BackendIntegrationTests.Utils.Helpers;
using Newtonsoft.Json;

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

    public async Task<string?> GetToken(string email, string password)
    {
        var data = new
        {
            email = email,
            password = password
        };

        var response = await ExecuteAsyncPost(ROUTE, data);
        dynamic? jsonResponse = JsonConvert.DeserializeObject(response.Content!);
        return jsonResponse?.token?.ToString();
    }
}