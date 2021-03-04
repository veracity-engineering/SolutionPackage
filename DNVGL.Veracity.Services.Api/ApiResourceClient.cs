using DNVGL.OAuth.Api.HttpClient;
using System;
using System.Net.Http;

namespace DNVGL.Veracity.Services.Api
{
    public abstract class ApiResourceClient
    {
        private readonly IOAuthHttpClientFactory _httpClientFactory;
        private readonly ISerializer _serializer;
        private readonly string _httpClientConfigurationName;

        private HttpClient _client;

        public ApiResourceClient(IOAuthHttpClientFactory httpClientFactory, ISerializer serializer, string clientConfigurationName)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _httpClientConfigurationName = clientConfigurationName;
        }

        protected HttpClient GetOrCreateHttpClient()
        {
            if (_client == null)
            {
                _client = _httpClientFactory.Create(_httpClientConfigurationName);
                _client.DefaultRequestHeaders.Add("Accept", "application/json");
            }
            return _client;
        }

        protected string Serialize<T>(T value) => _serializer.Serialize<T>(value);

        protected T Deserialize<T>(string value) => _serializer.Deserialize<T>(value);
    }
}
