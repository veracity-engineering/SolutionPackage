using System;
using System.Threading.Tasks;
using DNVGL.OAuth.Web.Abstractions;
using Microsoft.AspNetCore.Http;

namespace DNVGL.OAuth.Api.HttpClient.HttpClientHandlers
{
    internal class UserCredentialsHandler : BaseHttpClientHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IClientAppBuilder _appBuilder;
        private IClientApp _clientApp;

        public UserCredentialsHandler(OAuthHttpClientFactoryOptions options, IHttpContextAccessor httpContextAccessor, IClientAppBuilder appBuilder) : base(options)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _appBuilder = appBuilder ?? throw new ArgumentNullException(nameof(appBuilder));
        }

        protected override async Task<string> RetrieveToken()
        {
            var clientApp = GetOrCreateClientApp(_httpContextAccessor.HttpContext);
            var authResult = await clientApp.AcquireTokenSilent(_httpContextAccessor.HttpContext, _options.OpenIdConnectOptions.Scopes);
            return authResult.AccessToken;
        }

        private IClientApp GetOrCreateClientApp(HttpContext httpContext)
        {
            if (_clientApp != null)
                return _clientApp;
            _clientApp = _appBuilder
                .WithOpenIdConnectOptions(_options.OpenIdConnectOptions)
                .BuildForUserCredentials(httpContext);
            return _clientApp;
        }
    }
}
