using Microsoft.Identity.Client;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DNVGL.OAuth.UserCredentials.HttpClientHandlers
{
    internal class ClientCredentialsClientHandler : HttpClientHandler
    {
        private readonly OAuthHttpClientFactoryOptions _options;

        private IConfidentialClientApplication _confidentialClientApplication;

        public ClientCredentialsClientHandler(OAuthHttpClientFactoryOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage original, CancellationToken cancellationToken)
        {
            var token = await RetrieveToken();
            var request = AppendAuthHeaders(original, token, _options.SubscriptionKey);
            request.RequestUri = new Uri($"{_options.BaseUrl}{request.RequestUri.LocalPath}{request.RequestUri.Query}"); // Unfortunate hack, HttpClient.BaseAddress cannot carry path
            return await base.SendAsync(request, cancellationToken);
        }

        private static HttpRequestMessage AppendAuthHeaders(HttpRequestMessage original, string token, string subscriptionKey)
        {
            var request = original;
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Authorization", $"Bearer {token}");
            request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
            return request;
        }

        private async Task<string> RetrieveToken()
        {
            if (_confidentialClientApplication == null)
            {
                _confidentialClientApplication = ConfidentialClientApplicationBuilder.Create(_options.ClientId)
                    .WithB2CAuthority(_options.Authority)
                    .WithClientSecret(_options.ClientSecret)
                    .Build();
            }

            // User token cache?

            try
            {
                var authResult = await _confidentialClientApplication.AcquireTokenForClient(_options.Scopes).ExecuteAsync();
                return authResult.AccessToken;
            }
            catch (Exception)
            {
                // TODO: Handle
                throw;
            }
        }
    }
}
