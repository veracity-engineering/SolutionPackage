using DNVGL.OAuth.Web.Abstractions;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace DNVGL.OAuth.Api.HttpClient.HttpClientHandlers
{
    internal class UserCredentialsHandler : BaseHttpClientHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMsalAppBuilder _appBuilder;

        public UserCredentialsHandler(OAuthHttpClientFactoryOptions options, IHttpContextAccessor httpContextAccessor, IMsalAppBuilder appBuilder) : base(options)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _appBuilder = appBuilder;
        }

        protected override async Task<string> RetrieveToken()
        {
            var authResult = await _appBuilder.AcquireTokenSilent(_httpContextAccessor.HttpContext, _options.OpenIdConnectOptions.Scopes);
            return authResult.AccessToken;
        }
    }
}
