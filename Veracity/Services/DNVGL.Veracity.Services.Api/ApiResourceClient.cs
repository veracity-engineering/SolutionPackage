using DNVGL.OAuth.Api.HttpClient;
using DNVGL.Veracity.Services.Api.Exceptions;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api
{
	public abstract class ApiResourceClient
	{		
		private readonly ISerializer _serializer;		
		private readonly Lazy<HttpClient> _client;

		private readonly IHttpClientFactory _httpClientFactory;

		protected ApiResourceClient(IHttpClientFactory httpClientFactory, ISerializer serializer, OAuthHttpClientOptions option)
		{
			_httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));						
			_serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));			
			_client = new Lazy<HttpClient>(() =>
				{
					var clt = CreateClient(_httpClientFactory, option);
					clt.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ToAcceptMediaType(_serializer.DataFormat)));
					return clt;
				},
				LazyThreadSafetyMode.ExecutionAndPublication);
		}

		protected HttpClient Client => _client.Value;
				

		protected virtual HttpClient CreateClient(IHttpClientFactory factory, OAuthHttpClientOptions option)
		{
			return factory.CreateClient($"{option.Name}:{option.Flow}");
		}

		protected virtual string ToAcceptMediaType(DataFormat dataFormat)
		{
			switch (dataFormat)
			{
				case DataFormat.Json:
					return "application/json";
				case DataFormat.Xml:
					return "application/xml";
				default:
					throw new NotImplementedException($"Unknown {nameof(DataFormat)} type.");
			}
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

		protected async Task<T> ToResourceResult<T>(HttpRequestMessage request, bool isNotFoundNull = false)
		{
			var result =  await DoCallApi<T>(() => Client.SendAsync(request), isNotFoundNull);

			return result;
		}

		protected async Task ToResourceResult(HttpRequestMessage request)
		{
			var result = await DoCallApi(() => Client.SendAsync(request));
		}

		protected async Task<T> DoCallApi<T>(Func<Task<HttpResponseMessage>> doSend, bool ignoreNotFound = false)
		{
			var response = await DoCallApi(doSend, ignoreNotFound).ConfigureAwait(false);

			return await BuildResult<T>(response);
		}

		protected virtual async Task<HttpResponseMessage> DoCallApi(Func<Task<HttpResponseMessage>> doSend, bool ignoreNotFound = false)
		{
			var response =  await doSend().ConfigureAwait(false);

			await CheckResponse(response, ignoreNotFound);

			return response;
		}

		protected virtual async Task CheckResponse(HttpResponseMessage response, bool ignoreNotFound = false)
		{
			if (!response.IsSuccessStatusCode)
			{
				if (ignoreNotFound && response.StatusCode == HttpStatusCode.NotFound)
					return;

				throw await ServerErrorException.FromResponse(response);
			}
		}

		protected virtual async Task<T> BuildResult<T>(HttpResponseMessage response)
		{
			var result = response.IsSuccessStatusCode 
				? await DeserializeFromStream<T>(
					await response.Content.ReadAsStreamAsync().ConfigureAwait(false)).ConfigureAwait(false) 
				: default;

			return result;
		}

		protected string Serialize<T>(T value) => _serializer.Serialize(value);

		protected T? Deserialize<T>(string strValue) => _serializer.Deserialize<T>(strValue);

		protected Task SerializeToStream<T>(T value, Stream stream) => _serializer.SerializeAsync(value, stream);

		protected Task<T?> DeserializeFromStream<T>(Stream stream) => _serializer.DeserializeAsync<T>(stream);

		protected HttpContent ToJsonContent(string serializedContent) => new StringContent(serializedContent, Encoding.UTF8, "application/json");

		protected HttpContent ToJsonContent(object content) => ToJsonContent(Serialize(content));
	}
}
