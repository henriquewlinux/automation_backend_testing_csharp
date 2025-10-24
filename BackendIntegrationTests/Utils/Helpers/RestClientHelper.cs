using RestSharp;

namespace BackendIntegrationTests.Utils.Helpers
{
    public class RestClientHelper
    {
        private readonly RestClient _client;
        public RestClientHelper()
        {
            var options = new RestClientOptions(ApiConfig.BaseUrl)
            {
                MaxTimeout = (int)TimeSpan.FromSeconds(ApiConfig.TimeoutSeconds).TotalMilliseconds
            };
            _client = new RestClient(options);
        }


        public async Task<RestResponse> ExecuteAsyncPost(string route, Object data, Dictionary<string, string>? headers = null)
        {
            var request = new RestRequest(route, Method.Post);

            // Adiciona os headers se fornecidos
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.AddHeader(header.Key, header.Value);
                }
            }

            request.AddJsonBody(data);
            return await _client.ExecuteAsync(request);
        }

        public async Task<RestResponse> ExecuteAsyncGet(string route, Dictionary<string, string>? headers = null)
        {
            var request = new RestRequest(route, Method.Get);

            // Adiciona os headers se fornecidos
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.AddHeader(header.Key, header.Value);
                }
            }

            return await _client.ExecuteAsync(request);
        }


        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}