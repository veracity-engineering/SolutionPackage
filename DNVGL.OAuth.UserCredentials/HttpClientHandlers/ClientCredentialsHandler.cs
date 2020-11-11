using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace DNVGL.OAuth.Api.HttpClient.HttpClientHandlers
{
    internal class ClientCredentialsHandler : BaseHttpClientHandler
    {
        private IConfidentialClientApplication _confidentialClientApplication;

        public ClientCredentialsHandler(OAuthHttpClientFactoryOptions options) : base(options)
        {
        }

        protected override async Task<string> RetrieveToken()
        {
            if (_confidentialClientApplication == null)
            {
                _confidentialClientApplication = ConfidentialClientApplicationBuilder.Create(_options.OpenIdConnectOptions.ClientId)
                    .WithAuthority(_options.OpenIdConnectOptions.Authority)
                    .WithClientSecret(_options.OpenIdConnectOptions.ClientSecret)
                    .Build();
            }
            var authResult = await _confidentialClientApplication.AcquireTokenForClient(_options.OpenIdConnectOptions.Scopes).ExecuteAsync();
            return authResult.AccessToken;
        }
    }
}
