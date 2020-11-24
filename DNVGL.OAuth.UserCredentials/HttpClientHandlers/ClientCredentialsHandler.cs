using System;
using System.Threading.Tasks;
using DNVGL.OAuth.Web.Abstractions;

namespace DNVGL.OAuth.Api.HttpClient.HttpClientHandlers
{
    internal class ClientCredentialsHandler : BaseHttpClientHandler
    {
        private readonly IClientAppBuilder _appBuilder;
        private IClientApp _clientApp;

        public ClientCredentialsHandler(OAuthHttpClientFactoryOptions options, IClientAppBuilder appBuilder) : base(options)
        {
            _appBuilder = appBuilder ?? throw new ArgumentNullException(nameof(appBuilder));
        }

        protected override async Task<string> RetrieveToken()
        {
            var clientApp = GetOrCreateClientApp();
            var authResult = await clientApp.AcquireTokenForClient();
            return authResult.AccessToken;
        }

        private IClientApp GetOrCreateClientApp()
        {
            if (_clientApp != null)
                return _clientApp;
            _clientApp = _appBuilder
                .WithOpenIdConnectOptions(_options.OpenIdConnectOptions)
                .BuildForClientCredentials();
            return _clientApp;
        }
    }
}
