using System.Net.Http;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api
{
	class ApiClient : IApiClient
	{
		private readonly ISerializer _serializer;
		private readonly HttpClient _httpClient;

		internal protected HttpClient Client => _httpClient;

		public ISerializer Serializer => _serializer;

		public ApiClient(HttpClient client, ISerializer serializer)
		{
			_httpClient = client;
			_serializer = serializer;
		}

		public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
		{
			return await _httpClient.SendAsync(request);
		}
	}
}
