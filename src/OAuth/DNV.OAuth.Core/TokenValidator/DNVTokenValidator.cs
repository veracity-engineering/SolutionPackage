using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using DNV.OAuth.Abstractions.Constants;
using Microsoft.IdentityModel.Tokens;

namespace DNV.OAuth.Core.TokenValidator
{
	/// <summary>
	/// 
	/// </summary>
	public class DNVTokenValidator : JwtSecurityTokenHandler
	{
		private readonly Func<IEnumerable<Claim>, (bool Succeeded, string FailureReason)>? _customClaimsValidator;

		public DNVTokenValidator(Func<IEnumerable<Claim>, (bool Succeeded, string FailureReason)>? customClaimsValidator = null)
		{
			_customClaimsValidator = customClaimsValidator;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="token"></param>
		/// <param name="validationParameters"></param>
		/// <param name="validatedToken"></param>
		/// <returns></returns>
		/// <exception cref="SecurityTokenValidationException"></exception>
		public override ClaimsPrincipal ValidateToken(string token, TokenValidationParameters validationParameters,
			out SecurityToken validatedToken)
		{
			var principal = base.ValidateToken(token, validationParameters, out validatedToken);

			if (principal.Identity is ClaimsIdentity identity)
				ExtendIdentity(identity);

			if (_customClaimsValidator != null)
			{
				bool succeeded;
				string failureReason;

				try
				{
					(succeeded, failureReason) = _customClaimsValidator(principal.Claims);
				}
				catch (Exception ex)
				{
					throw new SecurityTokenValidationException(ex.Message);
				}

				if (!succeeded)
					throw new SecurityTokenValidationException(failureReason);
			}

			return principal;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="identity"></param>
		protected virtual void ExtendIdentity(ClaimsIdentity identity)
		{
			if (IsVersion1(identity))
				ExtendVersion1(identity);
			else
				ExtendVersion2(identity);
		}

		private static bool IsVersion1(ClaimsIdentity identity)
		{
			var verClaim = identity.FindFirst(c => c.Type == TokenClaimTypes.Version);

			return verClaim is { Value: "1.0" };
		}

		private static void ExtendVersion1(ClaimsIdentity identity)
		{
			var appIdClaim = identity.FindFirst(c => c.Type == TokenClaimTypes.AppId);

			if (appIdClaim != null)
			{
				identity.AddClaim(new Claim(TokenClaimTypes.FlowType, FlowTypeClaimValues.ClientFlow));
				identity.AddClaim(new Claim(TokenClaimTypes.RequestParty, appIdClaim.Value));
			}
			else
			{
				identity.AddClaim(new Claim(TokenClaimTypes.FlowType, FlowTypeClaimValues.UserFlow));
				var userIdClaim = identity.FindFirst(c => c.Type == ClaimTypes.NameIdentifier);
				if (userIdClaim != null)
					identity.AddClaim(new Claim(TokenClaimTypes.RequestParty, userIdClaim.Value));
			}
		}

		private static void ExtendVersion2(ClaimsIdentity identity)
		{
			var azpClaim = identity.FindFirst(c => c.Type == JwtRegisteredClaimNames.Azp);
			var audClaim = identity.FindFirst(c => c.Type == JwtRegisteredClaimNames.Aud);

			if (azpClaim != null 
			    && audClaim != null
				&& string.Equals(azpClaim.Value, audClaim.Value, StringComparison.OrdinalIgnoreCase))
			{
				identity.AddClaim(new Claim(TokenClaimTypes.FlowType, FlowTypeClaimValues.UserFlow));
				var subClaim = identity.FindFirst(c => c.Type == JwtRegisteredClaimNames.Sub);
				if (subClaim != null)
					identity.AddClaim(new Claim(TokenClaimTypes.RequestParty, subClaim.Value));
			}
			else
			{
				identity.AddClaim(new Claim(TokenClaimTypes.FlowType, FlowTypeClaimValues.ClientFlow));
				if (azpClaim != null)
					identity.AddClaim(new Claim(TokenClaimTypes.RequestParty, azpClaim.Value));
			}
		}
	}
}
