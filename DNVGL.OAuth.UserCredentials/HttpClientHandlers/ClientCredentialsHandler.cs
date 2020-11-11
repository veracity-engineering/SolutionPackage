using Microsoft.Identity.Client;
using System;
using System.Threading.Tasks;

namespace DNVGL.OAuth.Api.HttpClient.HttpClientHandlers
{
    internal class ClientCredentialsHandler : BaseHttpClientHandler
    {
        private readonly OAuthHttpClientFactoryOptions _options;

        private IConfidentialClientApplication _confidentialClientApplication;

        public ClientCredentialsHandler(OAuthHttpClientFactoryOptions options) : base(options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        protected override async Task<string> RetrieveToken()
        {
            if (_confidentialClientApplication == null)
            {
                _confidentialClientApplication = ConfidentialClientApplicationBuilder.Create(_options.OpenIdConnectOptions.ClientId)
                    .WithB2CAuthority(_options.OpenIdConnectOptions.Authority)
                    .WithClientSecret(_options.OpenIdConnectOptions.ClientSecret)
                    .Build();
            }
            var authResult = await _confidentialClientApplication.AcquireTokenForClient(_options.OpenIdConnectOptions.Scopes).ExecuteAsync();
            return authResult.AccessToken;
        }
    }
}
