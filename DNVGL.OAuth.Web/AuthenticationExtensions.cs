﻿using DNVGL.OAuth.Web.Abstractions;
using DNVGL.OAuth.Web.TokenCache;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
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

			var schemaOptions = new Dictionary<string, JwtOptions>();

			foreach (var section in sections)
			{
				var option = new JwtOptions();
				section.Bind(option);
				schemaOptions.Add(section.Key, option);
			}

			return builder.AddJwt(schemaOptions);
		}

		public static AuthenticationBuilder AddJwt(this AuthenticationBuilder builder, Action<Dictionary<string, JwtOptions>> setupAction)
		{
			if (setupAction == null)
			{
				throw new ArgumentNullException(nameof(setupAction));
			}

			var sections = new Dictionary<string, JwtOptions>();
			setupAction(sections);
			return builder.AddJwt(sections);
		}

		public static AuthenticationBuilder AddJwt(this AuthenticationBuilder builder, Dictionary<string, JwtOptions> schemaOptions)
		{
			if (schemaOptions == null || schemaOptions.Any())
			{
				throw new ArgumentNullException(nameof(schemaOptions));
			}

			foreach (var schemaOption in schemaOptions)
			{
				var option = schemaOption.Value;

				builder.AddJwtBearer(schemaOption.Key, o =>
				{
					o.Authority = option.Authority;

					if (option.TokenValidationParameters != null) o.TokenValidationParameters = option.TokenValidationParameters;

					if (option.Events != null) { o.Events = option.Events; }
				});
			}

			return builder;
		}
		#endregion

		#region AddOidc for Web App
		public static AuthenticationBuilder AddOidc(this IServiceCollection services, Action<OidcOptions> oidcSetupAction, Action<CookieAuthenticationOptions> cookieSetupAction = null)
		{
			if (oidcSetupAction == null)
			{
				throw new ArgumentNullException(nameof(oidcSetupAction));
			}

			var oidcOptions = new OidcOptions();
			oidcSetupAction(oidcOptions);
			return services.AddOidc(oidcOptions, cookieSetupAction);
		}
		public static AuthenticationBuilder AddOidc(this IServiceCollection services, OidcOptions oidcOptions, Action<CookieAuthenticationOptions> cookieSetupAction = null)
		{
			var builder = services.AddAuthentication(o =>
			{
				o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				o.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
			});

			builder.AddOidc(oidcOptions, cookieSetupAction);
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

		/// <summary>
		/// Add OpenId Connect authentication
		/// </summary>
		/// <param name="builder"></param>
		/// <param name="oidcOptions"></param>
		/// <param name="cookieSetupAction"></param>
		/// <returns></returns>
		public static AuthenticationBuilder AddOidc(this AuthenticationBuilder builder, OidcOptions oidcOptions, Action<CookieAuthenticationOptions> cookieSetupAction = null)
		{
			if (oidcOptions == null)
			{
				throw new ArgumentNullException(nameof(oidcOptions));
			}

			builder = cookieSetupAction != null ? builder.AddCookie(o => cookieSetupAction(o)) : builder.AddCookie();

			// switch to authorization code flow.
			if (oidcOptions.ResponseType == OpenIdConnectResponseType.Code)
			{
				builder.Services.AddDistributedTokenCache(oidcOptions);
			}

			builder.AddOpenIdConnect(o =>
			{
				o.Authority = oidcOptions.Authority;
				o.ClientId = oidcOptions.ClientId;
				o.ClientSecret = oidcOptions.ClientSecret;
				o.CallbackPath = oidcOptions.CallbackPath;
				o.ResponseType = oidcOptions.ResponseType ?? OpenIdConnectResponseType.IdToken;
				o.AuthenticationMethod = oidcOptions.AuthenticationMethod;
#if NETCORE3
				o.UsePkce = true;
#endif

				if (oidcOptions.Scopes == null || !oidcOptions.Scopes.Any()) oidcOptions.Scopes = new string[] { oidcOptions.ClientId };

				foreach (var scope in oidcOptions.Scopes) o.Scope.Add(scope);

				if (oidcOptions.Events != null) { o.Events = oidcOptions.Events; }

				if (o.AuthenticationMethod == OpenIdConnectRedirectBehavior.FormPost && o.Events.OnRedirectToIdentityProvider != null)
				{
					var onRedirectToIdentityProvider = o.Events.OnRedirectToIdentityProvider;

					o.Events.OnRedirectToIdentityProvider = context =>
					{
						context.Response.Headers.Remove("content-security-policy");
						return onRedirectToIdentityProvider(context);
					};
				}
			});

			return builder;
		}
		#endregion

		#region AddDistributedTokenCache
		private static void AddDistributedTokenCache(this IServiceCollection services, OidcOptions oidcOptions, Action<DistributedCacheEntryOptions> cacheSetupAction = null)
		{
			var cacheEntryOptions = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60) };
			cacheSetupAction?.Invoke(cacheEntryOptions);

			services.AddSingleton<ITokenCacheProvider>(f =>
			{
				var cache = f.GetRequiredService<IDistributedCache>();

				// add memory cache for token cache if no distributed cache set.
				if (cache == null)
				{
					services.AddDistributedMemoryCache();
					cache = f.GetRequiredService<IDistributedCache>();
				}

				var provider = new MsalTokenCacheProvider(cache, cacheEntryOptions);
				return provider;
			});

			async Task onCodeReceived(AuthorizationCodeReceivedContext context)
			{
				var msalClientApp = context.HttpContext.RequestServices.GetService<IClientAppBuilder>()
					.WithOpenIdConnectOptions(oidcOptions)
					.BuildForUserCredentials(context);
				await msalClientApp.AcquireTokenByAuthorizationCode(context);
			}

#if NETCORE2
			if (oidcOptions.Events == null) oidcOptions.Events = new OpenIdConnectEvents();
#else
			oidcOptions.Events ??= new OpenIdConnectEvents();
#endif

			if (oidcOptions.Events.OnAuthorizationCodeReceived == null)
			{
				oidcOptions.Events.OnAuthorizationCodeReceived = onCodeReceived;
			}
			else
			{
				var onAuthorizationCodeReceived = oidcOptions.Events.OnAuthorizationCodeReceived;

				oidcOptions.Events.OnAuthorizationCodeReceived = async context =>
				{
					await onCodeReceived(context);
					await onAuthorizationCodeReceived(context);
				};
			}

			services.AddSingleton(f =>
			{
				var appBuilder = new MsalClientAppBuilder(f.GetRequiredService<ITokenCacheProvider>()).WithOpenIdConnectOptions(oidcOptions);
				return appBuilder;
			});
		}
		#endregion
	}
}
