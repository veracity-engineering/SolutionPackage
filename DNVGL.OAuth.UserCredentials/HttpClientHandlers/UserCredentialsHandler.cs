using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DNVGL.OAuth.Api.HttpClient.HttpClientHandlers
{
    internal class UserCredentialsHandler : BaseHttpClientHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly OAuthHttpClientFactoryOptions _options;

        private IConfidentialClientApplication _confidentialClientApplication;

        public UserCredentialsHandler(OAuthHttpClientFactoryOptions options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        protected override async Task<string> RetrieveToken()
        {
            if (_confidentialClientApplication == null)
            {
                _confidentialClientApplication = ConfidentialClientApplicationBuilder.Create(_options.ClientId)
                    .WithB2CAuthority(_options.Authority)
                    .WithClientSecret(_options.ClientSecret)
                    .Build();

                var cacheManager = (IDistributedTokenCacheManager)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(IDistributedTokenCacheManager));
                cacheManager.SetCacheInstance(_confidentialClientApplication.UserTokenCache);
            }

            var accountIdentifier = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (accountIdentifier == null)
                throw new Exception($"Failed to retrieve account identifier from HttpContext user claims (claim type = '{ClaimTypes.NameIdentifier}').");

            var accounts = await _confidentialClientApplication.GetAccountsAsync();

            var account = await _confidentialClientApplication.GetAccountAsync(accountIdentifier);

            /*
            var acc = new SomeAccount
            {
                Username = "Missing from the token response",
                Environment = "login.microsoftonline.com",
                HomeAccountId = new AccountId(
                    "e0e00436-2622-436d-8af0-f46d312bb1aa-b2c_1a_signinwithadfsidp.ed815121-cdfa-4097-b524-e2b23cd36eb6",
                    "e0e00436-2622-436d-8af0-f46d312bb1aa-b2c_1a_signinwithadfsidp",
                    "ed815121-cdfa-4097-b524-e2b23cd36eb6"
                    )
            };
            */

            var authResult = await _confidentialClientApplication.AcquireTokenSilent(_options.Scopes, account).ExecuteAsync();
            //var authResult = await _confidentialClientApplication.AcquireTokenSilent(_options.Scopes, acc).ExecuteAsync();
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
