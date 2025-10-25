using RestSharp;
using BackendIntegrationTests.Utils;
using BackendIntegrationTests.Utils.Helpers;
using Newtonsoft.Json;

namespace BackendIntegrationTests.Routes;
public class LoginRoute : RestClientHelper
{
    const string ROUTE = "/login";
    public static async Task<RestResponse> LoginAsync(string email, string password)
    {
        var data = new
        {
            email = email,
            password = password
        };

        return await ExecuteAsyncPost(ROUTE, data);
    }

    public static async Task<string?> GetToken(string email, string password)
    {
        var data = new
        {
            email = email,
            password = password
        };

        var response = await ExecuteAsyncPost(ROUTE, data);

        var token = ExtractTokenFromResponse(response);
        return token;
    }

    public static string? ExtractTokenFromResponse(RestSharp.RestResponse response)
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