using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DNVGL.OAuth.Api.HttpClient.HttpClientHandlers
{
    internal abstract class BaseHttpClientHandler : HttpClientHandler
    {
        protected readonly OAuthHttpClientFactoryOptions _options;

        public BaseHttpClientHandler(OAuthHttpClientFactoryOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage original, CancellationToken cancellationToken)
        {
            var token = await RetrieveToken();
            var request = AppendAuthHeaders(original, token, _options.SubscriptionKey);
            return await base.SendAsync(request, cancellationToken);
        }

        private static HttpRequestMessage AppendAuthHeaders(HttpRequestMessage original, string token, string subscriptionKey)
        {
            var request = original;
            request.Headers.Add("Authorization", $"Bearer {token}");
            if (!string.IsNullOrEmpty(subscriptionKey))
                request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
            return request;
        }

        protected abstract Task<string> RetrieveToken();
    }
}
