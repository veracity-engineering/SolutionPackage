using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Client;
using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace DNVGL.OAuth.UserCredentials.HttpClientHandlers
{
    internal class UserCredentialsClientHandler : HttpClientHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly OAuthHttpClientFactoryOptions _options;

        private IConfidentialClientApplication _confidentialClientApplication;

        public UserCredentialsClientHandler(OAuthHttpClientFactoryOptions options, IHttpContextAccessor httpContextAccessor)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
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
                var accountIdentifier = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                var accounts = await _confidentialClientApplication.GetAccountsAsync();
                var account = await _confidentialClientApplication.GetAccountAsync(accountIdentifier);

                var authResult = await _confidentialClientApplication.AcquireTokenSilent(_options.Scopes, account).ExecuteAsync();

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
