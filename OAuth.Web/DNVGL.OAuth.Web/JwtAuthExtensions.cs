using DNV.OAuth.Core.TokenValidator;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DNVGL.OAuth.Web
{
	/// <summary>
	/// 
	/// </summary>
	public static class JwtAuthExtensions
	{
		public static AuthenticationBuilder AddJwtDefault(this IServiceCollection services) => services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);

		public static AuthenticationBuilder AddJwt(this AuthenticationBuilder builder, string authenticationSchema, Action<JwtOptions> setupAction)
		{
			if (setupAction == null)
			{
				throw new ArgumentNullException(nameof(setupAction));
			}

			var jwtOptions = new JwtOptions();
			setupAction(jwtOptions);
			return builder.AddJwt(authenticationSchema, jwtOptions);
		}

		public static AuthenticationBuilder AddJwt(this AuthenticationBuilder builder, string authenticationSchema, JwtOptions jwtOptions)
		{
			return builder.AddJwt(new Dictionary<string, JwtOptions> { { authenticationSchema, jwtOptions } });
		}

		public static AuthenticationBuilder AddJwt(this AuthenticationBuilder builder, IEnumerable<IConfigurationSection> sections)
		{
			if (sections == null || sections.Count() == 0)
			{
				throw new ArgumentNullException(nameof(sections));
			}

			var schemaOptions = new Dictionary<string, JwtOptions>();

			foreach (var section in sections)
			{
				var option = new JwtOptions();
				section.Bind(option);
				schemaOptions.Add(section.Key, option);
			}

			return builder.AddJwt(schemaOptions);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="builder"></param>
		/// <param name="schemaOptions"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static AuthenticationBuilder AddJwt(this AuthenticationBuilder builder, IDictionary<string, JwtOptions> schemaOptions)
		{
			if (schemaOptions == null || !schemaOptions.Any())
			{
				throw new ArgumentNullException(nameof(schemaOptions));
			}

			foreach (var schemaOption in schemaOptions)
			{
				var schemeNames = new List<string>();
				var jwtOptions = schemaOption.Value;

				if (!string.IsNullOrEmpty(jwtOptions.Authority))
				{
					var schemeName = schemaOption.Key;
					builder.AddJwtBearer(schemeName, o =>
					{
						o.Authority = jwtOptions.Authority;
						o.Audience = jwtOptions.ClientId;

						if (jwtOptions.TokenValidationParameters != null)
							o.TokenValidationParameters = jwtOptions.TokenValidationParameters;

						if (jwtOptions.Events != null)
							o.Events = jwtOptions.Events;

						o.SecurityTokenValidators.Clear();
						o.SecurityTokenValidators.Add(jwtOptions.SecurityTokenValidator ??
													  new DNVTokenValidator(jwtOptions.CustomClaimsValidator));
					});
					schemeNames.Add(schemeName);
				}

				if (jwtOptions.Authorities.Any())
					jwtOptions.Authorities.ForEach(aut =>
					{
						var schemeName = $"{schemaOption.Key}.{aut.SchemePostfix}";
						builder.AddJwtBearer(schemeName, o =>
						{
							o.Authority = aut.Authority;
							o.Audience = jwtOptions.ClientId;

							if (jwtOptions.TokenValidationParameters != null)
								o.TokenValidationParameters = jwtOptions.TokenValidationParameters;

							if (jwtOptions.Events != null)
								o.Events = jwtOptions.Events;

							o.SecurityTokenValidators.Clear();
							o.SecurityTokenValidators.Add(jwtOptions.SecurityTokenValidator ?? new DNVTokenValidator(jwtOptions.CustomClaimsValidator));
						});

						schemeNames.Add(schemeName);
					});

				if (schemeNames.Any())
					builder.Services.AddAuthorization(o =>
					{
						var policy = o.GetPolicy(jwtOptions.AuthorizationPolicyName);

						o.AddPolicy(jwtOptions.AuthorizationPolicyName,
							p =>
							{
								if (policy != null)
									p = p.Combine(policy);

								if (jwtOptions.AddAsDefault && o.DefaultPolicy != policy)
									p = p.Combine(o.DefaultPolicy);

								p.AddAuthenticationSchemes(schemeNames.ToArray()).RequireAuthenticatedUser();
							});

						if (jwtOptions.AddAsDefault)
							o.DefaultPolicy = o.GetPolicy(jwtOptions.AuthorizationPolicyName);
					});
			}

			return builder;
		}
	}
}
