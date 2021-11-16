using DNVGL.OAuth.Web.Abstractions;
using DNVGL.OAuth.Web.Exceptions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Client;
using System;
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
			
			var builder = _clientApp.AcquireTokenByAuthorizationCode(_scopes, authContext.ProtocolMessage.Code);
			var codeVerifier = authContext.TokenEndpointRequest.GetParameter("code_verifier");

			if (string.IsNullOrWhiteSpace(codeVerifier)) builder.WithPkceCodeVerifier(codeVerifier);

			var result = await builder.ExecuteAsync();
			authContext.HandleCodeRedemption(result.AccessToken, result.IdToken);
			return result;
		}

		public async Task<AuthenticationResult> AcquireTokenSilent(HttpContext httpContext)
		{
			var identifier = httpContext.User.GetMsalAccountId();
			var account = await _clientApp.GetAccountAsync(identifier);
			return await AcquireTokenSilent(account);
		}

		public async Task<AuthenticationResult> AcquireTokenSilent(IAccount account)
		{
			try
			{
				var builder = _clientApp.AcquireTokenSilent(_scopes, account);
				return await builder.ExecuteAsync();
			}
			catch (MsalUiRequiredException)
			{
				throw new TokenExpiredException();
			}
		}

		public async Task<AuthenticationResult> AcquireTokenForClient()
		{
			var builder = _clientApp.AcquireTokenForClient(_scopes);
			return await builder.ExecuteAsync();
		}

		public async Task ClearUserTokenCache(HttpContext httpContext)
		{
			var userAccount = await _clientApp.GetAccountAsync(httpContext.User.GetMsalAccountId());
			if (userAccount != null)
				await _clientApp.RemoveAsync(userAccount);
		}
	}
}
