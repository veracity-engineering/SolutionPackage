using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DNVGL.OAuth.UserCredentials
{
    public class OAuthHttpClientFactory : IOAuthHttpClientFactory
    {
        private readonly IEnumerable<OAuthHttpClientFactoryOptions> _options;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _baseUrl;

        public OAuthHttpClientFactory(IEnumerable<OAuthHttpClientFactoryOptions> options, IHttpContextAccessor httpContextAccessor)
        {
            _options = options;
            _httpContextAccessor = httpContextAccessor;
        }

        public HttpClient Create(string name)
        {
            var config = _options.First(o => o.Name.Equals(name));
            return BuildClient(config);
        }

        private HttpClient BuildClient(OAuthHttpClientFactoryOptions options)
        {
            return new HttpClient(new UserCredentialsClientHandler(_httpContextAccessor, options));
        }
    }

    internal class UserCredentialsClientHandler : HttpClientHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly OAuthHttpClientFactoryOptions _options;

        private IConfidentialClientApplication _confidentialClientApplication;

        public UserCredentialsClientHandler(IHttpContextAccessor httpContextAccessor, OAuthHttpClientFactoryOptions options)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
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
                //.WithRedirectUri(CallbackRedirectUri(context, options.CallbackPath))
                .WithClientSecret(_options.ClientSecret)
                .Build();
            }

            // User token cache?

            try
            {
                var account = await _confidentialClientApplication.GetAccountAsync(_httpContextAccessor.HttpContext.User.Identity.Name);
                //var authResult = await confidentialClientApplication.AcquireTokenByAuthorizationCode(options.Scopes, context.ProtocolMessage.Code).ExecuteAsync();
                var authResult = await _confidentialClientApplication.AcquireTokenSilent(_options.Scopes, account).ExecuteAsync();

                return authResult.AccessToken;
                // 'AccessToken' may be relayed as bearer token and made available to APIs
                //context.HandleCodeRedemption(authResult.AccessToken, authResult.IdToken);
            }
            catch (Exception)
            {
                // TODO: Handle
                throw;
            }
        }
    }

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
                    //.WithRedirectUri(CallbackRedirectUri(context, options.CallbackPath))
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

    interface IOAuthHttpClientFactory
    {
        HttpClient Create(string name);
    }

    interface ITokenProvider
    {
        Task<string> GetToken();

        Task RenewToken();
    }
}
