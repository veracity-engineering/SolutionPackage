using Microsoft.Identity.Client;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DNVGL.OAuth.Web.Abstractions
{
	public interface IClientApp
	{
		/// <summary>
		/// Attempts to authenticate using account retrieved from the specified context by calling <see cref="IConfidentialClientApplication.AcquireTokenByAuthorizationCode"/>.
		/// </summary>
		/// <param name="authCode"></param>
		/// <param name="codeVerifier"></param>
		/// <returns>Authentication result containing a token for the requested scopes.</returns>
		Task<AuthenticationResult> AcquireTokenByAuthorizationCode(string authCode, string codeVerifier = null);

		/// <summary>
		/// Attempts to authenticate using account retrieved from the <see cref="ClaimsPrincipal"/> by calling <see cref="IClientApplicationBase.AcquireTokenSilent"/>.
		/// </summary>
		/// <param name="claimsPrincipal"></param>
		/// <returns>Authentication result containing a token.</returns>
		Task<AuthenticationResult> AcquireTokenSilent(ClaimsPrincipal claimsPrincipal);

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
		/// Removes all tokens in the cache for the account retrieved from the specified <see cref="ClaimsPrincipal"/>.
		/// </summary>
		/// <param name="claimsPrincipal"></param>
		Task ClearUserTokenCache(ClaimsPrincipal claimsPrincipal);
	}
}
