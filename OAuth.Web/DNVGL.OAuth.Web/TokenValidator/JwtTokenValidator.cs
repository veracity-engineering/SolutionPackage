using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using DNVGL.OAuth.Web.Abstractions.Constants;
using Microsoft.IdentityModel.Tokens;

namespace DNVGL.OAuth.Web.TokenValidator
{
	public class JwtTokenValidator : JwtSecurityTokenHandler
	{
		private readonly Func<IEnumerable<Claim>, (bool Succeeded, string FailedReason)> _customClaimsValidator;

		public JwtTokenValidator(Func<IEnumerable<Claim>, (bool Succeeded, string FailedReason)> customClaimsValidator = null)
		{
			_customClaimsValidator = customClaimsValidator;
		}

		public override ClaimsPrincipal ValidateToken(string token, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
		{
			var principal = base.ValidateToken(token, validationParameters, out validatedToken);
			
			if (_customClaimsValidator == null) return principal;

			try
			{
				var jwt = ReadJwtToken(token);

				var (succeeded, failedReason) = _customClaimsValidator(jwt.Claims);

				if (!succeeded)
					throw new SecurityTokenValidationException(failedReason);

				return principal;
			}
			catch (SecurityTokenValidationException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new SecurityTokenValidationException(ex.Message);
			}
		}

		protected override JwtSecurityToken ValidateSignature(string token, TokenValidationParameters validationParameters)
		{
			var jwtToken = base.ValidateSignature(token, validationParameters);

			if (IsV1Token(jwtToken))
				ExtendV1Token(jwtToken);
			else 
				ExtendV2Token(jwtToken);

			return jwtToken;
		}

		private static bool IsV1Token(JwtSecurityToken token)
		{
			var verClaim = token.Claims.FirstOrDefault(c => c.Type == TokenClaimTypes.Version);

			return verClaim != null && verClaim.Value == "1.0";
		}

		private static void ExtendV1Token(JwtSecurityToken token)
		{
			var payload = token.Payload;

			var appIdClaim = token.Claims.FirstOrDefault(c => c.Type == TokenClaimTypes.AppId);

			if (appIdClaim != null)
			{
				payload.AddClaim(new Claim(TokenClaimTypes.PartyType, "client"));
				payload.AddClaim(new Claim(TokenClaimTypes.RequestParty, appIdClaim.Value));
			}
			else
			{
				payload.AddClaim(new Claim(TokenClaimTypes.PartyType, "user"));
				payload.AddClaim(new Claim(TokenClaimTypes.RequestParty, payload.Sub));
			}
		}

		private static void ExtendV2Token(JwtSecurityToken token)
		{
			var payload = token.Payload;

			var azpClaim = token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Azp);

			if (azpClaim != null && azpClaim.Value == token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Aud)?.Value)
			{
				payload.AddClaim(new Claim(TokenClaimTypes.PartyType, "user"));
				payload.AddClaim(new Claim(TokenClaimTypes.RequestParty, payload.Sub));
			}
			else
			{
				payload.AddClaim(new Claim(TokenClaimTypes.PartyType, "client"));
				payload.AddClaim(new Claim(TokenClaimTypes.RequestParty, payload.Azp));
			}
		}
	}
}
