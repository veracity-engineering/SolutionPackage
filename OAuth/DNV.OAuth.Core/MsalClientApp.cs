using System.Security.Claims;
using System.Threading.Tasks;
using DNV.OAuth.Abstractions;
using DNV.OAuth.Core.Exceptions;
using Microsoft.Extensions.Primitives;
using Microsoft.Identity.Client;

namespace DNV.OAuth.Core
{
	public class MsalClientApp : IClientApp
	{
		private readonly IConfidentialClientApplication _clientApp;
		private readonly StringValues _scope;

		public MsalClientApp(IConfidentialClientApplication confidentialClientApplication, StringValues scopes)
		{
			_clientApp = confidentialClientApplication;
			_scope = scopes;
		}

		public async Task<AuthenticationResult> AcquireTokenByAuthorizationCode(string authCode, string codeVerifier = null)
		{
			var builder = _clientApp.AcquireTokenByAuthorizationCode(_scope, authCode);

			if (!string.IsNullOrWhiteSpace(codeVerifier)) builder.WithPkceCodeVerifier(codeVerifier);

			var result = await builder.ExecuteAsync();
			return result;
		}

		public async Task<AuthenticationResult> AcquireTokenSilent(ClaimsPrincipal claimsPrincipal)
		{
			var identifier = claimsPrincipal.GetMsalAccountId();
			var account = await _clientApp.GetAccountAsync(identifier);
			return await AcquireTokenSilent(account);
		}

		public async Task<AuthenticationResult> AcquireTokenSilent(IAccount account)
		{
			try
			{
				var builder = _clientApp.AcquireTokenSilent(_scope, account);
				return await builder.ExecuteAsync();
			}
			catch (MsalUiRequiredException)
			{
				throw new TokenExpiredException();
			}
		}

		public async Task<AuthenticationResult> AcquireTokenForClient()
		{
			var builder = _clientApp.AcquireTokenForClient(_scope);
			return await builder.ExecuteAsync();
		}

		public async Task ClearUserTokenCache(ClaimsPrincipal claimsPrincipal)
		{
			var userAccount = await _clientApp.GetAccountAsync(claimsPrincipal.GetMsalAccountId());

			if (userAccount != null) await _clientApp.RemoveAsync(userAccount);
		}
	}
}
