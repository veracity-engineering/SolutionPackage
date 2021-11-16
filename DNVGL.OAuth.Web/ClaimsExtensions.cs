﻿using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace DNVGL.OAuth.Web
{
	/// <summary>
	/// 
	/// </summary>
	public static class ClaimsExtensions

	{
		/// <summary>
		/// Generates a MSAL Account Id from user claims and OIDC Options.
		/// </summary>
		/// <param name="claimsPrincipal"></param>
		/// <returns></returns>
		public static string GetMsalAccountId(this ClaimsPrincipal claimsPrincipal)
		{
			var objectId = claimsPrincipal.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier");
			var policy = claimsPrincipal.FindFirstValue("http://schemas.microsoft.com/claims/authnclassreference");
			var tenantId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier).Issuer.Split('/')[3];
			var msalAccountId = $"{objectId}-{policy}.{tenantId}";
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
