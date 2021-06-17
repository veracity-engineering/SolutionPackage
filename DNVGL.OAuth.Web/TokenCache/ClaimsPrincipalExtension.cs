using DNVGL.OAuth.Web.Abstractions;
using System.Security.Claims;

namespace DNVGL.OAuth.Web.TokenCache
{
	/// <summary>
	/// 
	/// </summary>
	public static class ClaimsPrincipalExtension
	{
		/// <summary>
		/// Generates a MSAL Account Id from user claims and OIDC Options.
		/// </summary>
		/// <param name="claimsPrincipal"></param>
		/// <param name="oidcOptions">The <see cref="OpenIdConnectOptions.TenantId">TenantId</see> of the options should be in GUID format.</param>
		/// <returns></returns>
		public static string GetHomeAccountId(this ClaimsPrincipal claimsPrincipal, OpenIdConnectOptions oidcOptions = null)
		{
			var objectId = claimsPrincipal.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier");
			var claim = claimsPrincipal.FindFirst("http://schemas.microsoft.com/claims/authnclassreference");
			var policy = string.IsNullOrWhiteSpace(oidcOptions?.SignInPolicy) ? claim.Value : oidcOptions.SignInPolicy;
			var tenantId = string.IsNullOrWhiteSpace(oidcOptions?.TenantId) ? claim.Issuer.Split('/')[3] : oidcOptions.TenantId;
			var msalAccountId = $"{objectId}-{policy}.{tenantId}";
			return msalAccountId?.ToLower();
		}

#if NETCORE2
		/// <summary>
		/// Gets the first match value of the specified claim.
		/// </summary>
		/// <param name="claimsPrincipal"></param>
		/// <param name="claimType"></param>
		/// <returns></returns>
		public static string FindFirstValue(this ClaimsPrincipal claimsPrincipal, string claimType)
		{
			var claim = claimsPrincipal.FindFirst(claimType);
			return claim?.Value;
		}
#endif
	}
}
