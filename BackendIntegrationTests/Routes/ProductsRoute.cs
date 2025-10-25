using RestSharp;
using BackendIntegrationTests.Utils.Helpers;
using BackendIntegrationTests.Utils;

namespace BackendIntegrationTests.Routes;
public class ProductsRoute : RestClientHelper
{
    const string ROUTE = "/products";

    public static async Task<RestResponse> GetAllProductsAsync(string token = "")
    {
        // Monta os headers usando o HeadersBuilder
        var headers = new HeadersBuilder()
            .WithContentType("application/json")
            .WithAccept("application/json");

        // Adiciona autorização apenas se o token for fornecido
        if (!string.IsNullOrEmpty(token))
        {
            headers.WithAuthorization(token);
        }

        // Executa a requisição GET com os headers
        return await ExecuteAsyncGet(ROUTE, headers.Build());
    }

    public static async Task<RestResponse> CreateProductAsync(object data, string token = "")
    {
        // Monta os headers usando o HeadersBuilder
        var headers = new HeadersBuilder()
            .WithContentType("application/json")
            .WithAccept("application/json");

        // Adiciona autorização apenas se o token for fornecido
        if (!string.IsNullOrEmpty(token))
        {
            headers.WithAuthorization(token);
        }

        // Executa a requisição POST com os headers
        return await ExecuteAsyncPost(ROUTE, data, headers.Build());
    }
}