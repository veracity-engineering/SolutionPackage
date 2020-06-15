using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DNVGL.OAuth.Common
{
	public static class OidcExtensions
	{
		public static IServiceCollection AddOidc(this IServiceCollection services, IConfiguration configuration, params string[] authSchemes)
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

		public static IServiceCollection AddOidc(this IServiceCollection services, Action<Dictionary<string, OidcOption>> setupAction)
		{
			var sections = new Dictionary<string, OidcOption>();
			setupAction(sections);

			var authBuilder = services.AddAuthentication();

			foreach (var section in sections)
			{
				var option = section.Value;

				authBuilder.AddJwtBearer(section.Key, o =>
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

			services.AddAuthorization();
			//services.AddAuthorization(options =>
			//{
			//	options.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
			//		.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
			//		.RequireAuthenticatedUser()
			//		.Build()
			//	);
			//});

			return services;
		}

		public static IApplicationBuilder UseOidc(this IApplicationBuilder app)
		{
			app.UseAuthentication();
			return app;
		}
	}
}
