using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Client;
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
            }

            var accountIdentifier = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (accountIdentifier == null)
                throw new Exception($"Failed to retrieve account identifier from HttpContext user claims (claim type = '{ClaimTypes.NameIdentifier}').");

            var account = await _confidentialClientApplication.GetAccountAsync(accountIdentifier);
            if (account == null)
                throw new Exception($"Failed to retrieve account by identifier '{accountIdentifier}' from ConfidentialClientApplication.");

            var authResult = await _confidentialClientApplication.AcquireTokenSilent(_options.Scopes, account).ExecuteAsync();
            return authResult.AccessToken;
        }
    }
}
