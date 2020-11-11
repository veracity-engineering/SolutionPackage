using System;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

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
                    //.WithAuthority("https://login.microsoftonline.com/dnvglb2ctest.onmicrosoft.com")
                    .WithClientSecret(_options.OpenIdConnectOptions.ClientSecret)
                    .Build();
            }
            var authResult = await _confidentialClientApplication.AcquireTokenForClient(_options.OpenIdConnectOptions.Scopes).ExecuteAsync();
            return authResult.AccessToken;
        }

        /*
        protected override async Task<string> RetrieveToken()
        {
            var context = new AuthenticationContext(_options.OpenIdConnectOptions.Authority);
            var credentials = new Microsoft.IdentityModel.Clients.ActiveDirectory.ClientCredential(_options.OpenIdConnectOptions.ClientId, _options.OpenIdConnectOptions.ClientSecret);
            var authResult = await context.AcquireTokenAsync(_options.OpenIdConnectOptions.Resource, credentials);
            return authResult.AccessToken;
        }
        */
    }
}
