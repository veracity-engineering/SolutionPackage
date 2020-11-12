using DNVGL.OAuth.Web.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Client;
using System.Threading.Tasks;

namespace DNVGL.OAuth.Web.TokenCache
{
    public class MsalClientApp : IClientApp
    {
        private readonly IConfidentialClientApplication _clientApp;
        private readonly Abstractions.OpenIdConnectOptions _oidcOptions;

        public MsalClientApp(IConfidentialClientApplication confidentialClientApplication, Abstractions.OpenIdConnectOptions openIdConnectOptions)
        {
            _clientApp = confidentialClientApplication;
            _oidcOptions = openIdConnectOptions;
        }

        public async Task<AuthenticationResult> AcquireTokenByAuthorizationCode<TOptions>(RemoteAuthenticationContext<TOptions> context) where TOptions : AuthenticationSchemeOptions
        {
            var authContext = context as AuthorizationCodeReceivedContext;
            authContext.HandleCodeRedemption();
            _clientApp.AppConfig.ExtraQueryParameters["code_verifier"] = authContext.TokenEndpointRequest.GetParameter("code_verifier");
            var result = await _clientApp.AcquireTokenByAuthorizationCode(_oidcOptions.Scopes, authContext.ProtocolMessage.Code).ExecuteAsync();
            authContext.HandleCodeRedemption(result.AccessToken, result.IdToken);
            return result;
        }

        public async Task<AuthenticationResult> AcquireTokenSilent(HttpContext httpContext, string[] scopes)
        {
            var account = await _clientApp.GetAccountAsync(httpContext.User.GetMsalAccountId(_oidcOptions));
            return await _clientApp.AcquireTokenSilent(scopes, account).ExecuteAsync();
        }

        public Task<AuthenticationResult> AcquireTokenForClient(string[] scopes)
        {
            return _clientApp.AcquireTokenForClient(scopes).ExecuteAsync();
        }

        public Task<IAccount> GetAccount(HttpContext httpContext)
        {
            return _clientApp.GetAccountAsync(httpContext.User.GetMsalAccountId(_oidcOptions));
        }

        public async Task ClearUserTokenCache(HttpContext httpContext)
        {
            var userAccount = await _clientApp.GetAccountAsync(httpContext.User.GetMsalAccountId(_oidcOptions));
            if (userAccount != null)
                await _clientApp.RemoveAsync(userAccount);
        }
    }
}
