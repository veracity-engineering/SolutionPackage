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
using System.Linq;
using System.Threading.Tasks;

namespace DNVGL.OAuth.Common
{
	public static class OidcExtensions
	{
		public static IServiceCollection AddOidc(this IServiceCollection services, IConfiguration configuration, params string[] authSchemes)
		{
			if(authSchemes == null || authSchemes.Length == 0)
			{
				throw new ArgumentNullException("No AuthenticationScheme is provided.");
			}

			var config = configuration.GetSection("OidcOptions");

			if (config == null)
			{
				throw new ArgumentNullException("Cannot find OidcOptions in appsettings.json.");
			}

			var oidcOptions = config.GetChildren();
			var authBuilder = services.AddAuthentication();

			foreach (var section in oidcOptions.Where(o => authSchemes.Contains(o.Key)))
			{
				var option = section.Get<OidcOption>();

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
							context.Response.StatusCode = StatusCodes.Status401Unauthorized;
							return Task.CompletedTask;
						}
					};
				});
			}

			services.AddAuthorization(options =>
			{
				options.DefaultPolicy = new AuthorizationPolicyBuilder()
					.RequireAuthenticatedUser()
					.AddAuthenticationSchemes(authSchemes)
					.Build();
			});

			return services;
		}

		public static IApplicationBuilder UseOidc(this IApplicationBuilder app)
		{
			return app.UseAuthentication().UseAuthorization();
		}
	}
}
