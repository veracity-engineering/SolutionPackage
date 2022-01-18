using DNVGL.OAuth.Api.HttpClient;
using DNVGL.Veracity.Services.Api.Exceptions;
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DNVGL.OAuth.Api.HttpClient.Extensions;
using Microsoft.Identity.Client;

namespace DNVGL.Veracity.Services.Api
{
	public abstract class ApiResourceClient
	{
		private readonly IOAuthHttpClientFactory _httpClientFactory;
		private readonly ISerializer _serializer;
		private readonly string _httpClientConfigurationName;
		private readonly Lazy<HttpClient> _client;

		protected ApiResourceClient(IOAuthHttpClientFactory httpClientFactory, ISerializer serializer, string clientConfigurationName)
		{
			_httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
			_serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
			_httpClientConfigurationName = clientConfigurationName;
			_client = new Lazy<HttpClient>(() =>
				{
					var clt = CreateClient(_httpClientFactory, _httpClientConfigurationName);
					clt.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ToAcceptMediaType(_serializer.DataFormat)));
					return clt;
				},
				LazyThreadSafetyMode.ExecutionAndPublication);
		}

		protected HttpClient Client => _client.Value;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="configurationName"></param>
		/// <returns></returns>
		protected virtual HttpClient CreateClient(IOAuthHttpClientFactory factory, string configurationName)
		{
			return factory.Create(c => c.Name == configurationName);
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
			var result = response.IsSuccessStatusCode ? Deserialize<T>(await response.Content.ReadAsStreamAsync().ConfigureAwait(false)) : default;

			return result;
		}

		protected string Serialize<T>(T value) => _serializer.Serialize(value);

		protected void Serialize<T>(T value, Stream stream) => _serializer.Serialize(stream);

		protected T Deserialize<T>(Stream stream) => _serializer.Deserialize<T>(stream);
	}
}
