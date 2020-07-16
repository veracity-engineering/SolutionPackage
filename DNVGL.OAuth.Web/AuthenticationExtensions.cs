using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DNVGL.OAuth.Web
{
	public static class AuthenticationExtensions
	{
		#region AddJwt for Web Api
		public static AuthenticationBuilder AddJwt(this AuthenticationBuilder builder, IEnumerable<IConfigurationSection> sections)
		{
			if (sections == null)
			{
				throw new ArgumentNullException(nameof(sections));
			}

			var schemaOptions = new Dictionary<string, OidcOption>();

			foreach (var section in sections)
			{
				var option = new OidcOption();
				section.Bind(option);
				schemaOptions.Add(section.Key, option);
			}

			return builder.AddJwt(schemaOptions);
		}

		public static AuthenticationBuilder AddJwt(this AuthenticationBuilder builder, Action<Dictionary<string, OidcOption>> setupAction)
		{
			if (setupAction == null)
			{
				throw new ArgumentNullException(nameof(setupAction));
			}

			var sections = new Dictionary<string, OidcOption>();
			setupAction(sections);
			return builder.AddJwt(sections);
		}

		public static AuthenticationBuilder AddJwt(this AuthenticationBuilder builder, Dictionary<string, OidcOption> schemaOptions)
		{
			if (schemaOptions == null || schemaOptions.Count() == 0)
			{
				throw new ArgumentNullException(nameof(schemaOptions));
			}

			foreach (var schemaOption in schemaOptions)
			{
				var option = schemaOption.Value;

				builder.AddJwtBearer(schemaOption.Key, o =>
				{
					var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(option.MetadataAddress, new OpenIdConnectConfigurationRetriever());
					o.ConfigurationManager = configManager;
					o.Authority = option.Authority;
					o.Audience = option.ClientId;
					o.TokenValidationParameters = new TokenValidationParameters { ValidateIssuerSigningKey = true };
				});
			}

			return builder;
		}
		#endregion

		#region AddOidc for Web App
		public static AuthenticationBuilder AddOidc(this IServiceCollection services, Action<OidcOption> setupAction)
		{
			var builder = services.AddAuthentication(o =>
			{
				o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				o.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
			});

			builder.AddOidc(setupAction);
			return builder;
		}

		public static AuthenticationBuilder AddOidc(this AuthenticationBuilder builder, Action<OidcOption> setupAction)
		{
			if (setupAction == null)
			{
				throw new ArgumentNullException(nameof(setupAction));
			}

			var option = new OidcOption();
			setupAction(option);
			return builder.AddOidc(option);
		}

		public static AuthenticationBuilder AddOidc(this AuthenticationBuilder builder, OidcOption option)
		{
			if (option == null)
			{
				throw new ArgumentNullException(nameof(option));
			}

			builder.AddCookie().AddOpenIdConnect(o =>
			{
				o.ConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(option.MetadataAddress, new OpenIdConnectConfigurationRetriever());
				o.Authority = option.Authority;
				o.ClientId = option.ClientId;
				o.CallbackPath = option.CallbackPath;
				o.ResponseType = option.ResponseType;

				if (option.Scopes != null)
				{
					option.Scopes.ToList().ForEach(s => o.Scope.Add(s));
				}

				// switch to authorization code flow.
				if (o.ResponseType == OpenIdConnectResponseType.Code)
				{
					o.ClientSecret = option.ClientSecret;
				}

				if (option.Events != null) { o.Events = option.Events; }

				// sample code to intecept token response and to add tokens to cache.
				o.Events.OnTokenResponseReceived = async context =>
				{
					var tokenResponse = context.TokenEndpointResponse;
					var cache = context.HttpContext.RequestServices.GetService<IDistributedCache>();
					await cache.SetStringAsync("access_token", tokenResponse.AccessToken);
					await cache.SetStringAsync("refresh_token", tokenResponse.RefreshToken);
				};
			});

			return builder;
		}
		#endregion
	}
}
