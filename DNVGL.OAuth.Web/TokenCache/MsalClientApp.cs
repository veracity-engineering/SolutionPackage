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

		public async Task<AuthenticationResult> AcquireTokenSilent(HttpContext httpContext)
		{
			var account = await this.GetAccount(httpContext);
			return await _clientApp.AcquireTokenSilent(_oidcOptions.Scopes, account).ExecuteAsync();
		}

		public async Task<AuthenticationResult> AcquireTokenForClient()
		{
			return await _clientApp.AcquireTokenForClient(_oidcOptions.Scopes).ExecuteAsync();
		}

		public async Task<IAccount> GetAccount(HttpContext httpContext)
		{
			return await _clientApp.GetAccountAsync(httpContext.User.GetHomeAccountId(_oidcOptions));
		}

		public async Task ClearUserTokenCache(HttpContext httpContext)
		{
			var account = await this.GetAccount(httpContext);
			if (account != null) await _clientApp.RemoveAsync(account);
		}
	}
}
