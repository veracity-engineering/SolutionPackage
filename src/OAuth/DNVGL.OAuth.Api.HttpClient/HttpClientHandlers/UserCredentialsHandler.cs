using System;
using System.Threading.Tasks;
using DNV.OAuth.Abstractions;
using Microsoft.AspNetCore.Http;


namespace DNVGL.OAuth.Api.HttpClient.HttpClientHandlers
{
    internal class UserCredentialsHandler : BaseHttpClientHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Lazy<IClientApp> _clientApp;

        public UserCredentialsHandler(OAuthHttpClientOptions option, IHttpContextAccessor httpContextAccessor, IClientAppBuilder appBuilder) : base(option)
        {
	        _httpContextAccessor = httpContextAccessor;
	        _clientApp = new Lazy<IClientApp>(() => appBuilder.Build(OAuthOptions));
        }

        protected override async Task<string> RetrieveToken()
        {
            var user = _httpContextAccessor.HttpContext.User;
            var authResult = await _clientApp.Value.AcquireTokenSilent(user);
            return authResult.AccessToken;
        }
    }
}
