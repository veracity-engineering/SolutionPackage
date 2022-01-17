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
            var user = _httpContextAccessor.HttpContext.User;
            var clientApp = GetOrCreateClientApp();
            var authResult = await clientApp.AcquireTokenSilent(user);
            return authResult.AccessToken;
        }

        private IClientApp GetOrCreateClientApp()
        {
            if (_clientApp != null) return _clientApp;
            
            _clientApp = _appBuilder.BuildWithOptions(_options.OAuthClientOptions);
            return _clientApp;
        }
    }
}
