namespace BackendIntegrationTests.Utils.Helpers
{
    public class HeadersBuilder
    {
        private readonly Dictionary<string, string> _headers = new Dictionary<string, string>();

        public HeadersBuilder WithContentType(string contentType)
        {
            _headers["Content-Type"] = contentType;
            return this;
        }

        public HeadersBuilder WithAuthorization(string token)
        {
            _headers["Authorization"] = token;
            return this;
        }

        public HeadersBuilder WithAccept(string accept)
        {
            _headers["Accept"] = accept;
            return this;
        }

        public Dictionary<string, string> Build()
        {
            return this._headers;
        }
    }
}
