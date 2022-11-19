using DNV.OAuth.Abstractions;
using DNV.OAuth.Core;
using DNV.OAuth.Core.TokenValidator;
using DNVGL.OAuth.Web.Oidc;
using DNVGL.OAuth.Web.TokenCache;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Threading.Tasks;

namespace DNVGL.OAuth.Web
{
	public static class OidcAuthExtensions
	{

		#region AddOidc for Web App
		public static AuthenticationBuilder AddOidc(
			this IServiceCollection services,
			Action<OidcOptions>? oidcSetupAction,
			Action<CookieAuthenticationOptions>? cookieSetupAction = null
		) => services.AddDefaultAuthentication().AddOidc(oidcSetupAction, cookieSetupAction);

		public static AuthenticationBuilder AddOidc(
			this IServiceCollection services,
			OidcOptions? oidcOptions,
			Action<CookieAuthenticationOptions>? cookieSetupAction = null
		) => services.AddDefaultAuthentication().AddOidc(oidcOptions, cookieSetupAction);

		public static AuthenticationBuilder AddOidc(
			this AuthenticationBuilder builder,
			Action<OidcOptions>? oidcSetupAction,
			Action<CookieAuthenticationOptions>? cookieSetupAction = null
		)
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
		public static AuthenticationBuilder AddOidc(
			this AuthenticationBuilder builder,
			OidcOptions? oidcOptions,
			Action<CookieAuthenticationOptions>? cookieSetupAction = null
		)
		{
			if (oidcOptions == null) throw new ArgumentNullException(nameof(oidcOptions));

			var services = builder.Services;
			services.AddSingleton<OAuth2Options>(oidcOptions);
			oidcOptions.Initialize();

			builder.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, cookieSetupAction);

			builder.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme,
				o =>
				{
					o.Authority = oidcOptions.Authority;
					o.ClientId = oidcOptions.ClientId;
					o.ClientSecret = oidcOptions.ClientSecret;
					o.CallbackPath = oidcOptions.CallbackPath;
					o.ResponseType = oidcOptions.ResponseType;
					o.AuthenticationMethod = oidcOptions.AuthenticationMethod;
					o.UsePkce = true;
					o.Scope.Add(oidcOptions.Scope);
					ConfigureSecurityTokenValidator(oidcOptions, o);
					ConfigureEvents(oidcOptions, o);
				});

			return builder;
		}

		private static AuthenticationBuilder AddDefaultAuthentication(this IServiceCollection services) =>
			services.AddAuthentication(o =>
			{
				o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				o.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
			});

		private static void ConfigureSecurityTokenValidator(OidcOptions oidcOptions, OpenIdConnectOptions o)
		{
			o.SecurityTokenValidator = oidcOptions.SecurityTokenValidator ?? new DNVTokenValidator();
		}

		private static void ConfigureEvents(OidcOptions oidcOptions, OpenIdConnectOptions o)
		{
			if (oidcOptions.Events != null) o.Events = oidcOptions.Events;

			if (o.AuthenticationMethod == OpenIdConnectRedirectBehavior.FormPost)
			{
				var onRedirectToIdp = o.Events.OnRedirectToIdentityProvider;

				o.Events.OnRedirectToIdentityProvider = context =>
				{
					context.ProtocolMessage.EnsureCspForOidcFormPostBehavior();
					return onRedirectToIdp(context);
				};

				var onRedirectToIdpSignOut = o.Events.OnRedirectToIdentityProviderForSignOut;

				o.Events.OnRedirectToIdentityProviderForSignOut = context =>
				{
					context.ProtocolMessage.EnsureCspForOidcFormPostBehavior();
					return onRedirectToIdpSignOut(context);
				};
			}
		}

		#endregion

		#region Add token cache

		/// <summary>
		/// Add session token caches.
		/// </summary>
		/// <param name="app"></param>
		/// <param name="useDataProtection"></param>
		/// <returns></returns>
		/// <remarks>Ensure to add <seealso cref="SessionMiddlewareExtensions.UseSession(IApplicationBuilder)"/> before <see cref="AuthAppBuilderExtensions.UseAuthentication"/>.</remarks>
		public static AuthenticationBuilder AddSessionTokenCaches(this AuthenticationBuilder app, bool useDataProtection = true)
		{
			var services = app.Services;
			services.AddSession(o => o.Cookie.IsEssential = true);
			services.AddHttpContextAccessor();
			services.TryAddSingleton<ICacheStorage, SessionCacheStorage>();
			services.AddTokenCaches(useDataProtection);
			services.HandleAuthorizationCode();
			return app;
		}

		/// <summary>
		/// Adds in memory token caches
		/// </summary>
		/// <param name="app"></param>
		/// <param name="cacheConfigAction"></param>
		/// <param name="useDataProtection"></param>
		/// <returns></returns>
		public static AuthenticationBuilder AddInMemoryTokenCaches(
			this AuthenticationBuilder app,
			Action<MemoryCacheEntryOptions>? cacheConfigAction = null,
			bool useDataProtection = true
		)
		{
			var services = app.Services;
			services.AddInMemoryTokenCaches(cacheConfigAction, useDataProtection);
			services.HandleAuthorizationCode();
			return app;
		}

		/// <summary>
		/// Adds distributed token caches
		/// </summary>
		/// <param name="app"></param>
		/// <param name="cacheConfigAction"></param>
		/// <param name="useDataProtection"></param>
		/// <returns></returns>
		public static AuthenticationBuilder AddDistributedTokenCaches(
			this AuthenticationBuilder app,
			Action<DistributedCacheEntryOptions>? cacheConfigAction = null,
			bool useDataProtection = true
		)
		{
			var services = app.Services;
			services.AddDistributedTokenCaches(cacheConfigAction, useDataProtection);
			services.HandleAuthorizationCode();
			return app;
		}

		private static void HandleAuthorizationCode(this IServiceCollection services)
		{
			services.TryAddSingleton<IClientAppFactory>(p =>
			{
				var oauth2Options = p.GetRequiredService<OAuth2Options>();
				var tokenCacheProvider = p.GetRequiredService<ITokenCacheProvider>();
				return new MsalClientAppFactory(oauth2Options, tokenCacheProvider);
			});

			services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, o =>
			{
				var handler = o.Events.OnAuthorizationCodeReceived;

				o.Events.OnAuthorizationCodeReceived = async context =>
				{
					await OnCodeReceived(context).ConfigureAwait(false);
					await handler(context).ConfigureAwait(false);
				};
			});

			static async Task OnCodeReceived(AuthorizationCodeReceivedContext context)
			{
				var serviceProvider = context.HttpContext.RequestServices;
				var oauth2Options = serviceProvider.GetRequiredService<OAuth2Options>();

				var codeVerifier = context.TokenEndpointRequest.GetParameter("code_verifier");
				var authCode = context.TokenEndpointRequest.Code;
				var clientAppFactory = serviceProvider.GetRequiredService<IClientAppFactory>();
				var clientApp = clientAppFactory.CreateForUser(oauth2Options.Scope);

				context.HandleCodeRedemption();
				var result = await clientApp.AcquireTokenByAuthorizationCode(authCode, codeVerifier);
				context.HandleCodeRedemption(result.AccessToken, result.IdToken);
			}
		}

		#endregion
	}
}
