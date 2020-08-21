using System;
using System.Security.Claims;

namespace DNVGL.OAuth.Demo.TokenCache
{
	public static class ClaimsPrincipalExtension
	{
		private const string MsalAccountIdClaimType = "msalAccountId";

		public static string GetMsalAccountIdClaimType(this ClaimsPrincipal claimsPrincipal) => MsalAccountIdClaimType;

		public static string GetMsalAccountId(this ClaimsPrincipal claimsPrincipal)
		{
			return claimsPrincipal.FindFirstValue(MsalAccountIdClaimType);
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

		//public static string GetTenantId(ClaimsPrincipal claimsPrincipal)
		//{
		//	var tenantId = claimsPrincipal.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid");

		//	if (string.IsNullOrEmpty(tenantId))
		//	{
		//		tenantId = claimsPrincipal.FindFirstValue("tid");
		//	}

		//	return tenantId;
		//}
	}
}
