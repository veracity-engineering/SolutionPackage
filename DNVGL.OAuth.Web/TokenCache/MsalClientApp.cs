using DNVGL.OAuth.Web.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Client;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNVGL.OAuth.Web.TokenCache
{
    public class MsalClientApp : IClientApp
    {
        private readonly IConfidentialClientApplication _clientApp;
        private readonly IEnumerable<string> _scopes;

        public MsalClientApp(IConfidentialClientApplication confidentialClientApplication, IEnumerable<string> scopes)
        {
            _clientApp = confidentialClientApplication;
            _scopes = scopes;
        }

        public async Task<AuthenticationResult> AcquireTokenByAuthorizationCode<TOptions>(RemoteAuthenticationContext<TOptions> context) where TOptions : AuthenticationSchemeOptions
        {
            var authContext = context as AuthorizationCodeReceivedContext;
            authContext.HandleCodeRedemption();
            _clientApp.AppConfig.ExtraQueryParameters["code_verifier"] = authContext.TokenEndpointRequest.GetParameter("code_verifier");
            var result = await _clientApp.AcquireTokenByAuthorizationCode(_scopes, authContext.ProtocolMessage.Code).ExecuteAsync();
            authContext.HandleCodeRedemption(result.AccessToken, result.IdToken);
            return result;
        }

        public async Task<AuthenticationResult> AcquireTokenSilent(HttpContext httpContext)
        {
            var account = await _clientApp.GetAccountAsync(httpContext.User.GetMsalAccountId());
            return await _clientApp.AcquireTokenSilent(_scopes, account).ExecuteAsync();
        }

        public Task<AuthenticationResult> AcquireTokenForClient()
        {
            return _clientApp.AcquireTokenForClient(_scopes).ExecuteAsync();
        }

        public Task<IAccount> GetAccount(HttpContext httpContext)
        {
            return _clientApp.GetAccountAsync(httpContext.User.GetMsalAccountId());
        }

        public async Task ClearUserTokenCache(HttpContext httpContext)
        {
            var userAccount = await _clientApp.GetAccountAsync(httpContext.User.GetMsalAccountId());
            if (userAccount != null)
                await _clientApp.RemoveAsync(userAccount);
        }
    }
}
