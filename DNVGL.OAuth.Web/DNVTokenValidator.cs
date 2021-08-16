using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DNVGL.OAuth.Web
{
	public class DNVTokenValidator : JwtSecurityTokenHandler
	{
		private readonly Func<IEnumerable<Claim>, bool> _customValidation;

		public DNVTokenValidator(Func<IEnumerable<Claim>, bool> customValidation = null)
		{
			_customValidation = customValidation;
		}

		public override ClaimsPrincipal ValidateToken(string token, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
		{
			var principal = base.ValidateToken(token, validationParameters, out validatedToken);

			if (_customValidation == null) return principal;

			try
			{
				var jwt = this.ReadJwtToken(token);
				if (_customValidation(jwt.Claims)) return principal;
			}
			catch (Exception ex)
			{
				throw new SecurityTokenValidationException(ex.Message);
			}

			throw new SecurityTokenValidationException("Custom validation failed");

		}
	}
}
