using DNVGL.Veracity.Services.Api.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.Extensions
{
	public static class IApiResourceClientExtensions
	{
		public static HttpContent ToJsonContent(this IApiResourceClient client, string serializedContent)
		{ 
			return new StringContent(serializedContent, Encoding.UTF8, "application/json");
		}

		public static HttpContent ToJsonContent(this IApiResourceClient client, object content)
		{
			return client.ToJsonContent(client.Serialize(content));
		}

		public static string Serialize<T>(this IApiResourceClient client, T value) => client.Serializer.Serialize(value);

		public static T? Deserialize<T>(this IApiResourceClient client, string strValue) => client.Serializer.Deserialize<T>(strValue);

		public static Task SerializeToStream<T>(this IApiResourceClient client, T value, Stream stream) => client.Serializer.SerializeAsync(value, stream);

		public static Task<T?> DeserializeFromStream<T>(this IApiResourceClient client, Stream stream) => client.Serializer.DeserializeAsync<T>(stream);


		public static async Task<T> ToResourceResult<T>(this IApiResourceClient client, HttpRequestMessage request, bool isNotFoundNull = false, Func<HttpResponseMessage, Task<T>>? buildResult = null, Func<HttpResponseMessage, bool, Task>? checkResponse = null)
		{
			var result = await client.DoCallApi<T>(() => client.SendAsync(request), isNotFoundNull, buildResult, checkResponse);

			return result;
		}

		public static async Task ToResourceResult(this IApiResourceClient client, HttpRequestMessage request)
		{
			var result = await client.DoCallApi(() => client.SendAsync(request));
		}

		public static async Task<T> DoCallApi<T>(this IApiResourceClient client, Func<Task<HttpResponseMessage>> doSend, bool ignoreNotFound = false, Func<HttpResponseMessage, Task<T>>? buildResult = null, Func<HttpResponseMessage, bool, Task>? checkResponse = null)
		{
			var response = await client.DoCallApi(doSend, ignoreNotFound, checkResponse).ConfigureAwait(false);

			if (buildResult != null)
			{
				return await buildResult(response);
			}
			else
			{
				var result = response.IsSuccessStatusCode
					? await client.DeserializeFromStream<T>(
						await response.Content.ReadAsStreamAsync().ConfigureAwait(false)).ConfigureAwait(false)
					: default;

				return result;
			}
		}

		public static async Task<HttpResponseMessage> DoCallApi(this IApiResourceClient client, Func<Task<HttpResponseMessage>> doSend, bool ignoreNotFound = false, Func<HttpResponseMessage, bool, Task>? checkResponse = null)
		{
			var response = await doSend().ConfigureAwait(false);

			if (checkResponse == null)
			{
				if (!response.IsSuccessStatusCode)
				{
					if (ignoreNotFound && response.StatusCode == HttpStatusCode.NotFound)
						return response;

					throw await ServerErrorException.FromResponse(response);
				}
			}
			else
			{
				await checkResponse(response, ignoreNotFound);
			}

			return response;
		}

		public static Task<T> GetResource<T>(this IApiResourceClient client, string requestUri, bool isNotFoundNull = true) =>
	client.ToResourceResult<T>(new HttpRequestMessage(HttpMethod.Get, requestUri), isNotFoundNull);

		public static Task<T> PostResource<T>(this IApiResourceClient client,  string requestUri, HttpContent content, bool isNotFoundNull = true) =>
			client.ToResourceResult<T>(new HttpRequestMessage(HttpMethod.Post, requestUri) { Content = content }, isNotFoundNull);

		public static Task<T> PutResource<T>(this IApiResourceClient client,  string requestUri, HttpContent content, bool isNotFoundNull = true) =>
			client.ToResourceResult<T>(new HttpRequestMessage(HttpMethod.Put, requestUri) { Content = content }, isNotFoundNull);

		public static Task PutResource(this IApiResourceClient client, string requestUri, HttpContent content) =>
			client.ToResourceResult(new HttpRequestMessage(HttpMethod.Put, requestUri) { Content = content });

		public static Task DeleteResource(this IApiResourceClient client, string requestUri) =>
			client.ToResourceResult(new HttpRequestMessage(HttpMethod.Delete, requestUri));

#if NETSTANDARD2_0
		public static Task PatchResource(this IApiResourceClient client, string requestUri) =>
			client.ToResourceResult(new HttpRequestMessage(new HttpMethod("Patch"), requestUri));
#endif

#if NETSTANDARD2_1_OR_GREATER
		public static Task PatchResource(this IApiResourceClient client, string requestUri) =>
			client.ToResourceResult(new HttpRequestMessage(HttpMethod.Patch, requestUri));
#endif

	}
}
