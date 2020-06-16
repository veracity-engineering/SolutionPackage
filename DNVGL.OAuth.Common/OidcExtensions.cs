using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNVGL.OAuth.Common
{
	public static class OidcExtensions
	{
		public static AuthenticationBuilder AddOidc(this AuthenticationBuilder services, IConfiguration configuration, params string[] authSchemes)
		{
			if (authSchemes == null || authSchemes.Length == 0)
			{
				throw new ArgumentNullException("No AuthenticationScheme is provided.");
			}

			var config = configuration.GetSection("OidcOptions");

			if (config == null)
			{
				throw new ArgumentNullException("Cannot find OidcOptions in appsettings.json.");
			}

			return services.AddOidc(o =>
			{
				foreach (var section in config.GetChildren())
				{
					o.Add(section.Key, section.Get<OidcOption>());
				}
			});
		}
		public static AuthenticationBuilder AddOidc(this AuthenticationBuilder services, params IConfigurationSection[] sections)
		{
			if (sections == null || sections.Length == 0)
			{
				throw new ArgumentNullException("No AuthenticationScheme is provided.");
			}

			return services.AddOidc(o =>
			{
				foreach (var section in sections)
				{
					o.Add(section.Key, section.Get<OidcOption>());
				}
			});
		}

		public static AuthenticationBuilder AddOidc(this AuthenticationBuilder builder, Action<Dictionary<string, OidcOption>> setupAction)
		{
			var sections = new Dictionary<string, OidcOption>();
			setupAction(sections);

			foreach (var section in sections)
			{
				var option = section.Value;

				builder.AddJwtBearer(section.Key, o =>
				{
					var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(option.MetadataAddress, new OpenIdConnectConfigurationRetriever());
					o.ConfigurationManager = configManager;
					o.Authority = option.Authority;
					o.Audience = option.ClientId;
					o.TokenValidationParameters = new TokenValidationParameters { ValidateIssuerSigningKey = true };

					o.Events = new JwtBearerEvents
					{
						OnAuthenticationFailed = context =>
						{
							return Task.CompletedTask;
						},
						OnChallenge = context =>
						{
							return Task.CompletedTask;
						}
					};
				});
			}

			return builder;
		}
	}
}
