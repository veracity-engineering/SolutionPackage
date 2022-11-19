using DNV.OAuth.Abstractions.Constants;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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
			var appId = identity.FindFirst(c => c.Type == TokenClaimTypes.AppId)?.Value;

			if (appId != null)
			{
				identity.AddClaim(new Claim(TokenClaimTypes.FlowType, FlowTypeClaimValues.ClientFlow));
				identity.AddClaim(new Claim(TokenClaimTypes.RequestParty, appId));
			}
			else
			{
				identity.AddClaim(new Claim(TokenClaimTypes.FlowType, FlowTypeClaimValues.UserFlow));
				var userId = identity.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

				if (userId != null)
					identity.AddClaim(new Claim(TokenClaimTypes.RequestParty, userId));
			}
		}

		private static void ExtendVersion2(ClaimsIdentity identity)
		{
			var azp = identity.FindFirst(c => c.Type == JwtRegisteredClaimNames.Azp)?.Value;
			var aud = identity.FindFirst(c => c.Type == JwtRegisteredClaimNames.Aud)?.Value;

			if (string.Equals(azp, aud, StringComparison.OrdinalIgnoreCase))
			{
				identity.AddClaim(new Claim(TokenClaimTypes.FlowType, FlowTypeClaimValues.UserFlow));
				var userId = identity.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

				if (userId != null)
					identity.AddClaim(new Claim(TokenClaimTypes.RequestParty, userId));
			}
			else
			{
				identity.AddClaim(new Claim(TokenClaimTypes.FlowType, FlowTypeClaimValues.ClientFlow));

				if (azp != null)
					identity.AddClaim(new Claim(TokenClaimTypes.RequestParty, azp));
			}
		}
	}
}
