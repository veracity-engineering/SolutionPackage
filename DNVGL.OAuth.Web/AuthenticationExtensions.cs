using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;

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

			var schemaOptions = new Dictionary<string, OidcOptions>();

			foreach (var section in sections)
			{
				var option = new OidcOptions();
				section.Bind(option);
				schemaOptions.Add(section.Key, option);
			}

			return builder.AddJwt(schemaOptions);
		}

		public static AuthenticationBuilder AddJwt(this AuthenticationBuilder builder, Action<Dictionary<string, OidcOptions>> setupAction)
		{
			if (setupAction == null)
			{
				throw new ArgumentNullException(nameof(setupAction));
			}

			var sections = new Dictionary<string, OidcOptions>();
			setupAction(sections);
			return builder.AddJwt(sections);
		}

		public static AuthenticationBuilder AddJwt(this AuthenticationBuilder builder, Dictionary<string, OidcOptions> schemaOptions)
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
		public static AuthenticationBuilder AddOidc(this IServiceCollection services, Action<OidcOptions> oidcSetupAction, Action<CookieAuthenticationOptions> cookieSetupAction = null)
		{
			var builder = services.AddAuthentication(o =>
			{
				o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				o.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
			});

			builder.AddOidc(oidcSetupAction, cookieSetupAction);
			return builder;
		}

		public static AuthenticationBuilder AddOidc(this AuthenticationBuilder builder, Action<OidcOptions> oidcSetupAction, Action<CookieAuthenticationOptions> cookieSetupAction = null)
		{
			if (oidcSetupAction == null)
			{
				throw new ArgumentNullException(nameof(oidcSetupAction));
			}

			var oidcOptions = new OidcOptions();
			oidcSetupAction(oidcOptions);
			return builder.AddOidc(oidcOptions, cookieSetupAction);
		}

		public static AuthenticationBuilder AddOidc(this AuthenticationBuilder builder, OidcOptions oidcOptions, Action<CookieAuthenticationOptions> cookieSetupAction = null)
		{
			if (oidcOptions == null)
			{
				throw new ArgumentNullException(nameof(oidcOptions));
			}

			builder = cookieSetupAction != null ? builder.AddCookie(o => cookieSetupAction(o)) : builder.AddCookie();

			builder.AddOpenIdConnect(o =>
			{
				o.ConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(oidcOptions.MetadataAddress, new OpenIdConnectConfigurationRetriever());
				o.Authority = oidcOptions.Authority;
				o.ClientId = oidcOptions.ClientId;
				o.CallbackPath = oidcOptions.CallbackPath;
				o.ResponseType = oidcOptions.ResponseType ?? OpenIdConnectResponseType.IdToken;
#if NETCORE3
				o.UsePkce = true;
#endif

				if (oidcOptions.Scopes != null)
				{
					oidcOptions.Scopes.ToList().ForEach(s => o.Scope.Add(s));
				}

				// switch to authorization code flow.
				if (o.ResponseType == OpenIdConnectResponseType.Code)
				{
					o.ClientSecret = oidcOptions.ClientSecret;
				}

				if (oidcOptions.Events != null) { o.Events = oidcOptions.Events; }

				// sample code to intecept token response and to add tokens to cache.
				var onTokenResponseReceived = o.Events.OnTokenResponseReceived;

				o.Events.OnTokenResponseReceived = async context =>
				{
					var tokenResponse = context.TokenEndpointResponse;

					if (tokenResponse != null)
					{
						var cache = context.HttpContext.RequestServices.GetService<IDistributedCache>();
						await cache.SetStringAsync("access_token", tokenResponse.AccessToken ?? string.Empty);
						await cache.SetStringAsync("refresh_token", tokenResponse.RefreshToken ?? string.Empty);
					}

					if (onTokenResponseReceived != null)
					{
						await onTokenResponseReceived(context);
					}
				};
			});

			return builder;
		}
		#endregion
	}
}
