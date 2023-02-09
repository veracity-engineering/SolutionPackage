using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DNV.OAuth.Abstractions;
using DNVGL.OAuth.Api.HttpClient.Exceptions;

namespace DNVGL.OAuth.Api.HttpClient.HttpClientHandlers
{
	internal abstract class BaseHttpClientHandler : DelegatingHandler
	{
		protected readonly OAuthHttpClientOptions _option;

		protected BaseHttpClientHandler(OAuthHttpClientOptions option)
		{
			_option = option;
		}

		protected BaseHttpClientHandler(OAuthHttpClientOptions option, HttpMessageHandler handler) : base(handler)
		{
			_option = option;
		}

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			var token = await RetrieveToken();
			if (string.IsNullOrEmpty(token))
				throw new MissingTokenException();
			PopulateAuthHeader(request, token);
			PopulateSubKeyHeader(request);

			return await base.SendAsync(request, cancellationToken);
		}

		protected virtual void PopulateAuthHeader(HttpRequestMessage request, string accessToken)
		{
			request.Headers.Add("Authorization", $"Bearer {accessToken}");
		}

		protected string SubscriptionKey => _option.SubscriptionKey;

		protected virtual void PopulateSubKeyHeader(HttpRequestMessage request)
		{
			if (!string.IsNullOrEmpty(SubscriptionKey))
				request.Headers.Add("Ocp-Apim-Subscription-Key", SubscriptionKey);
		}

		protected OAuth2Options OAuthOptions => _option.OAuthClientOptions;

		protected string Authority => _option.OAuthClientOptions.Authority;

		protected abstract Task<string> RetrieveToken();
	}
}
