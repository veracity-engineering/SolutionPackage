using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace DNVGL.OAuth.Web
{
	public static class ClaimsExtensions
	{
		public struct ClaimTypes
		{
			public static readonly string ObjectId = "http://schemas.microsoft.com/identity/claims/objectidentifier";
			public static readonly string Policy = "http://schemas.microsoft.com/claims/authnclassreference";
			public static readonly string NameIdentifier = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
		}

		/// <summary>
		/// Generates a MSAL Account Id from user claims and OIDC Options.
		/// </summary>
		/// <param name="claimsPrincipal"></param>
		/// <returns></returns>
		public static string GetMsalAccountId(this ClaimsPrincipal claimsPrincipal)
		{
			var objectId = claimsPrincipal.FindFirst(ClaimTypes.ObjectId);
			var policy = claimsPrincipal.FindFirstValue(ClaimTypes.Policy);
			var tenantId = objectId.Issuer.Split('/')[3];
			var msalAccountId = $"{objectId.Value}-{policy}.{tenantId}";
			return msalAccountId.ToLower();
		}

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

		/// <summary>
		/// Gets the first match value of the specified claim.
		/// </summary>
		/// <param name="jwtToken"></param>
		/// <param name="claimType"></param>
		/// <returns></returns>
		public static string FindFirstValue(this JwtSecurityToken jwtToken, string claimType)
		{
			var claim = jwtToken.Claims.FirstOrDefault(c => c.Type == claimType);
			return claim?.Value;
		}
	}
}
