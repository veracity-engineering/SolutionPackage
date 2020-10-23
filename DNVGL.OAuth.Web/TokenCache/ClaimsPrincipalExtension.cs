using System.Security.Claims;

namespace DNVGL.OAuth.Web.TokenCache
{
	public static class ClaimsPrincipalExtension
	{
		/// <summary>
		/// Generates a MSAL Account Id from user claims and OIDC Options.
		/// </summary>
		/// <param name="claimsPrincipal"></param>
		/// <param name="oidcOptions">The <see cref="OidcOptions.TenantId">TenantId</see> of the options should be in GUID format.</param>
		/// <returns></returns>
		public static string GetMsalAccountId(this ClaimsPrincipal claimsPrincipal, OidcOptions oidcOptions)
		{
			var objectId = claimsPrincipal.GetObjectId();
			var tenantId = oidcOptions.TenantId;
			var policy = oidcOptions.SignInPolicy;
			var msalAccountId = $"{objectId}-{policy}.{tenantId}";
			return msalAccountId?.ToLower();
		}

#if !NETCORE3
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

		/// <summary>
		/// Gets the <see href="http://schemas.microsoft.com/identity/claims/objectidentifier">oid</see> from user claims.
		/// </summary>
		/// <param name="claimsPrincipal"></param>
		/// <returns></returns>
		public static string GetObjectId(this ClaimsPrincipal claimsPrincipal)
		{
			var objectId = claimsPrincipal.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier");

			if (string.IsNullOrEmpty(objectId))
			{
				objectId = claimsPrincipal.FindFirstValue("oid");
			}

			return objectId;
		}
	}
}
