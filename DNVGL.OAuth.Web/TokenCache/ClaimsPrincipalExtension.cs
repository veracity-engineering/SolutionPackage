using System.Security.Claims;

namespace DNVGL.OAuth.Web.TokenCache
{
	public static class ClaimsPrincipalExtension
	{
		public static string GetMsalAccountId(this ClaimsPrincipal claimsPrincipal, OidcOptions oidcOptions)
		{
			var objectId = claimsPrincipal.GetObjectId();
			var tenantId = oidcOptions.TenantId;
			var policy = oidcOptions.SignInPolicy;
			var msalAccountId = $"{objectId}-{policy}.{tenantId}";
			return msalAccountId;
		}

#if !NETCORE3
		public static string FindFirstValue(this ClaimsPrincipal claimsPrincipal, string claimType)
		{
			var claim = claimsPrincipal.FindFirst(claimType);
			return claim?.Value;
		}
#endif

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
