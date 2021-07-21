using DNVGL.OAuth.Api.HttpClient;
using DNVGL.Veracity.Services.Api.Exceptions;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api
{
	public abstract class ApiResourceClient
	{
		private readonly IOAuthHttpClientFactory _httpClientFactory;
		private readonly ISerializer _serializer;
		private readonly string _httpClientConfigurationName;

		private HttpClient _client;

		protected ApiResourceClient(IOAuthHttpClientFactory httpClientFactory, ISerializer serializer, string clientConfigurationName)
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

		protected Task<T> GetResource<T>(string requestUri, bool isNotFoundNull = true) =>
			ToResourceResult<T>(new HttpRequestMessage(HttpMethod.Get, requestUri), isNotFoundNull);

		protected Task<T> PostResource<T>(string requestUri, HttpContent content, bool isNotFoundNull = true) =>
			ToResourceResult<T>(new HttpRequestMessage(HttpMethod.Post, requestUri) { Content = content }, isNotFoundNull);

		protected Task<T> PutResource<T>(string requestUri, HttpContent content, bool isNotFoundNull = true) =>
			ToResourceResult<T>(new HttpRequestMessage(HttpMethod.Put, requestUri) { Content = content }, isNotFoundNull);

		protected Task PutResource(string requestUri, HttpContent content) =>
			ToResourceResult(new HttpRequestMessage(HttpMethod.Put, requestUri) { Content = content });

		protected Task DeleteResource(string requestUri) =>
			ToResourceResult(new HttpRequestMessage(HttpMethod.Delete, requestUri));

		protected async Task<T> ToResourceResult<T>(HttpRequestMessage request, bool isNotFoundNull)
		{
			var response = await GetOrCreateHttpClient().SendAsync(request);
			if (isNotFoundNull)
			{
				if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
					return default;
			}
			try
			{
				response.EnsureSuccessStatusCode();
				var content = await response.Content.ReadAsStringAsync();
				return Deserialize<T>(content);
			}
			catch (HttpRequestException exception)
			{
				throw await ServerErrorException.FromResponse(response, exception);
			}
		}

		protected async Task ToResourceResult(HttpRequestMessage request)
		{
			var response = await GetOrCreateHttpClient().SendAsync(request);
			try
			{
				response.EnsureSuccessStatusCode();
			}
			catch (HttpRequestException exception)
			{
				throw await ServerErrorException.FromResponse(response, exception);
			}
		}

		protected string Serialize<T>(T value) => _serializer.Serialize<T>(value);

		protected T Deserialize<T>(string value) => _serializer.Deserialize<T>(value);
	}
}
