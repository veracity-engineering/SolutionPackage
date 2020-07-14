﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DNVGL.OAuth.Common
{
	public static class AuthenticationExtensions
	{
		#region AddJwt for Web Api
		public static AuthenticationBuilder AddJwt(this AuthenticationBuilder builder, IConfiguration configuration, params string[] authSchemes)
		{
			if (authSchemes == null || authSchemes.Length == 0)
			{
				throw new ArgumentNullException("No AuthenticationScheme is provided.");
			}

			var config = configuration as IConfigurationSection ?? configuration.GetSection("OidcOptions");

			if (config == null)
			{
				throw new ArgumentNullException("Cannot find OidcOptions in appsettings.json.");
			}

			return builder.AddJwt(o =>
			{
				foreach (var section in config.GetChildren().Where(s => authSchemes.Contains(s.Key)))
				{
					o.Add(section.Key, section.Get<OidcOption>());
				}
			});
		}

		public static AuthenticationBuilder AddJwt(this AuthenticationBuilder builder, params IConfigurationSection[] sections)
		{
			if (sections == null || sections.Length == 0)
			{
				throw new ArgumentNullException("sections");
			}

			var options = sections.ToDictionary(s => s.Key, s => s.Get<OidcOption>());
			return builder.AddJwt(options);
		}

		public static AuthenticationBuilder AddJwt(this AuthenticationBuilder builder, Action<Dictionary<string, OidcOption>> setupAction)
		{
			if (setupAction == null)
			{
				throw new ArgumentNullException("setupAction");
			}

			var sections = new Dictionary<string, OidcOption>();
			setupAction(sections);
			return builder.AddJwt(sections);
		}

		public static AuthenticationBuilder AddJwt(this AuthenticationBuilder builder, Dictionary<string, OidcOption> sections)
		{
			if (sections == null || sections.Count == 0)
			{
				throw new ArgumentNullException("sections");
			}

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

					//o.Events = new JwtBearerEvents
					//{
					//	OnAuthenticationFailed = context =>
					//	{
					//		return Task.CompletedTask;
					//	},
					//	OnChallenge = context =>
					//	{
					//		return Task.CompletedTask;
					//	}
					//};
				});
			}

			return builder;
		}
		#endregion

		#region AddOidc for Web App
		public static AuthenticationBuilder AddOidc(this AuthenticationBuilder builder, IConfigurationSection section, OpenIdConnectEvents events = null)
		{
			if (section == null)
			{
				throw new ArgumentNullException("section");
			}

			return builder.AddOidc(section.Get<OidcOption>(), events);
		}

		public static AuthenticationBuilder AddOidc(this AuthenticationBuilder builder, Action<OidcOption> setupAction, OpenIdConnectEvents events = null)
		{
			if (setupAction == null)
			{
				throw new ArgumentNullException("setupAction");
			}

			var option = new OidcOption();
			setupAction(option);
			return builder.AddOidc(option, events);
		}

		public static AuthenticationBuilder AddOidc(this AuthenticationBuilder builder, OidcOption option, OpenIdConnectEvents events = null)
		{
			if (option == null)
			{
				throw new ArgumentNullException("option");
			}

			builder.AddCookie().AddOpenIdConnect(o =>
			{
				o.ConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(option.MetadataAddress, new OpenIdConnectConfigurationRetriever());
				o.Authority = option.Authority;
				o.ClientId = option.ClientId;
				o.CallbackPath = option.CallbackPath;

				// switch to authorization code flow if client secret exist.
				if(!string.IsNullOrWhiteSpace(option.ClientSecret))
				{
					o.ClientSecret = option.ClientSecret;
					o.ResponseType = OpenIdConnectResponseType.Code;

					// set to true to store tokens into cookies.
					o.SaveTokens = false;

					if(option.Scopes != null)
					{
						option.Scopes.ToList().ForEach(s => o.Scope.Add(s));
					}

					// offline_access scope is required to retreive refresh token.
					o.Scope.Add(OpenIdConnectScope.OfflineAccess);
				}

				if (events != null) { o.Events = events; }

				// sample code of intecepting token response.
				o.Events.OnTokenResponseReceived = context => {
					var tokenResponse = context.TokenEndpointResponse;
					context.HttpContext.Response.Headers.Add("access_token", tokenResponse.AccessToken);
					context.HttpContext.Response.Headers.Add("refresh_token", tokenResponse.RefreshToken);
					return Task.CompletedTask;
				};
			});

			return builder;
		}

		public static AuthenticationBuilder AddOidc(this IServiceCollection services, IConfigurationSection section, OpenIdConnectEvents events = null)
		{
			if (section == null)
			{
				throw new ArgumentNullException("section");
			}

			return services.AddOidc(section.Get<OidcOption>(), events);
		}

		public static AuthenticationBuilder AddOidc(this IServiceCollection services, Action<OidcOption> setupAction, OpenIdConnectEvents events = null)
		{
			if (setupAction == null)
			{
				throw new ArgumentNullException("setupAction");
			}

			var option = new OidcOption();
			setupAction(option);
			return services.AddOidc(setupAction, events);
		}

		public static AuthenticationBuilder AddOidc(this IServiceCollection services, OidcOption option, OpenIdConnectEvents events = null)
		{
			if (option == null)
			{
				throw new ArgumentNullException("option");
			}

			var builder = services.AddAuthentication(o =>
			{
				o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				o.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
			});

			builder.AddOidc(option, events);
			return builder;
		}
		#endregion
	}
}
