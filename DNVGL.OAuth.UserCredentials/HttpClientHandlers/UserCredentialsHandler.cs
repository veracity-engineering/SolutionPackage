using DNVGL.OAuth.Api.HttpClient.TokenCache;
using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Client;
using System;
using System.Threading.Tasks;

namespace DNVGL.OAuth.Api.HttpClient.HttpClientHandlers
{
    internal class UserCredentialsHandler : BaseHttpClientHandler
    {
        private readonly OAuthHttpClientFactoryOptions _options;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMsalTokenCacheProvider _tokenCacheProvider;

        private IConfidentialClientApplication _confidentialClientApplication;

        public UserCredentialsHandler(OAuthHttpClientFactoryOptions options, IHttpContextAccessor httpContextAccessor, IMsalTokenCacheProvider tokenCacheProvider) : base(options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _tokenCacheProvider = tokenCacheProvider;
        }

        protected override async Task<string> RetrieveToken()
        {
            if (_confidentialClientApplication == null)
            {
                _confidentialClientApplication = ConfidentialClientApplicationBuilder.Create(_options.OpenIdConnectOptions.ClientId)
                    .WithB2CAuthority(_options.OpenIdConnectOptions.Authority)
                    .WithClientSecret(_options.OpenIdConnectOptions.ClientSecret)
                    .Build();

                if (_tokenCacheProvider != null)
                    await _tokenCacheProvider.InitializeAsync(_confidentialClientApplication.UserTokenCache);
            }

            var accountId = _httpContextAccessor.HttpContext.User.GetMsalAccountId(_options.OpenIdConnectOptions.TenantId, _options.OpenIdConnectOptions.SignInPolicy);
            var account = await _confidentialClientApplication.GetAccountAsync(accountId);
            var authResult = await _confidentialClientApplication.AcquireTokenSilent(_options.OpenIdConnectOptions.Scopes, account).ExecuteAsync();
            return authResult.AccessToken;
        }

        private class SomeAccount : IAccount
        {
            public string Username { get; set; }

            public string Environment { get; set; }

            public AccountId HomeAccountId { get; set; }
        }
    }
}
