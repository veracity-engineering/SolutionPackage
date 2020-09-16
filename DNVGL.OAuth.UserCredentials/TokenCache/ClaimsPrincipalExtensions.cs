using System.Security.Claims;

namespace DNVGL.OAuth.Api.HttpClient.TokenCache
{
    public static class ClaimsPrincipalExtensions
	{
		public static string GetMsalAccountId(this ClaimsPrincipal claimsPrincipal, string tenantId, string signInPolicy)
		{
			var objectId = claimsPrincipal.GetObjectId();
			var msalAccountId = $"{objectId}-{signInPolicy}.{tenantId}";
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
