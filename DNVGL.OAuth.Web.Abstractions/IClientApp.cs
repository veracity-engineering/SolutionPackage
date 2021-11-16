using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Client;
using System.Threading.Tasks;

namespace DNVGL.OAuth.Web.Abstractions
{
	public interface IClientApp
	{
		/// <summary>
		/// Attempts to authenticate using account retrieved from the specified context by calling <see cref="IConfidentialClientApplication.AcquireTokenByAuthorizationCode"/>.
		/// </summary>
		/// <typeparam name="TOptions"></typeparam>
		/// <param name="context"></param>
		/// <returns>Authentication result containing a token for the requested scopes.</returns>
		Task<AuthenticationResult> AcquireTokenByAuthorizationCode<TOptions>(RemoteAuthenticationContext<TOptions> context) where TOptions : AuthenticationSchemeOptions;

		/// <summary>
		/// Attempts to authenticate using account retrieved from the <see cref="HttpContext"/> by calling <see cref="IClientApplicationBase.AcquireTokenSilent"/>.
		/// </summary>
		/// <param name="httpContext"></param>
		/// <returns>Authentication result containing a token.</returns>
		Task<AuthenticationResult> AcquireTokenSilent(HttpContext httpContext);

		/// <summary>
		/// Attempts to authenticate using account by calling <see cref="IClientApplicationBase.AcquireTokenSilent"/>.
		/// </summary>
		/// <param name="account"></param>
		/// <returns>Authentication result containing a token.</returns>
		Task<AuthenticationResult> AcquireTokenSilent(IAccount account);

		/// <summary>
		/// Attempts to authenticate using client credentials by calling <see cref="IConfidentialClientApplication.AcquireTokenForClient"/>.
		/// </summary>
		/// <returns>Authentication result containing a token.</returns>
		Task<AuthenticationResult> AcquireTokenForClient();

		/// <summary>
		/// Removes all tokens in the cache for the account retrieved from the specified <see cref="HttpContext"/>.
		/// </summary>
		/// <param name="httpContext"></param>
		Task ClearUserTokenCache(HttpContext httpContext);
	}
}
