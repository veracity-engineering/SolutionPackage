using DNV.OAuth.Abstractions;
using DNV.OAuth.Core;
using DNV.OAuth.Core.TokenCache;
using DNV.OAuth.Core.TokenValidator;
using DNVGL.OAuth.Web.Oidc;
using DNVGL.OAuth.Web.TokenCache;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DNVGL.OAuth.Web
{
	/// <summary>
	/// 
	/// </summary>
	public static class OAuthExtensions
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

		public static AuthenticationBuilder AddJwt(this AuthenticationBuilder builder, string authenticationSchema, JwtOptions jwtOptions)
		{
			return builder.AddJwt(new Dictionary<string, JwtOptions> { { authenticationSchema, jwtOptions } });
		}

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

		public static AuthenticationBuilder AddJwt(this AuthenticationBuilder builder, Action<IDictionary<string, JwtOptions>> setupAction)
		{
			if (setupAction == null)
			{
				throw new ArgumentNullException(nameof(setupAction));
			}

			var sections = new Dictionary<string, JwtOptions>();
			setupAction(sections);
			return builder.AddJwt(sections);
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

		#endregion

		#region AddOidc for Web App
		public static AuthenticationBuilder AddOidc(
			this IServiceCollection services,
			Action<OidcOptions> oidcSetupAction,
			Action<CookieAuthenticationOptions> cookieSetupAction = null,
			Action<DistributedCacheEntryOptions> cacheSetupAction = null
		)
		{
			if (oidcSetupAction == null)
			{
				throw new ArgumentNullException(nameof(oidcSetupAction));
			}

			var oidcOptions = new OidcOptions();
			oidcSetupAction(oidcOptions);
			return services.AddOidc(oidcOptions, cookieSetupAction, cacheSetupAction);
		}

		public static AuthenticationBuilder AddOidc(
			this IServiceCollection services,
			OidcOptions oidcOptions,
			Action<CookieAuthenticationOptions> cookieSetupAction = null,
			Action<DistributedCacheEntryOptions> cacheSetupAction = null
		)
		{
			var builder = services.AddAuthentication(o =>
			{
				o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				o.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
			});

			return builder.AddOidc(oidcOptions, cookieSetupAction, cacheSetupAction);
		}

		public static AuthenticationBuilder AddOidc(
			this AuthenticationBuilder builder,
			Action<OidcOptions> oidcSetupAction,
			Action<CookieAuthenticationOptions> cookieSetupAction = null,
			Action<DistributedCacheEntryOptions> cacheSetupAction = null
		)
		{
			if (oidcSetupAction == null)
			{
				throw new ArgumentNullException(nameof(oidcSetupAction));
			}

			var oidcOptions = new OidcOptions();
			oidcSetupAction(oidcOptions);
			return builder.AddOidc(oidcOptions, cookieSetupAction, cacheSetupAction);
		}

		/// <summary>
		/// Add OpenId Connect authentication
		/// </summary>
		/// <param name="builder"></param>
		/// <param name="oidcOptions"></param>
		/// <param name="cookieSetupAction"></param>
		/// <param name="cacheSetupAction"></param>
		/// <returns></returns>
		public static AuthenticationBuilder AddOidc(
			this AuthenticationBuilder builder,
			OidcOptions oidcOptions,
			Action<CookieAuthenticationOptions> cookieSetupAction = null,
			Action<DistributedCacheEntryOptions> cacheSetupAction = null
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
#if !NETCORE2
					o.UsePkce = true;
#endif

					o.Scope.Add(oidcOptions.Scope);
					ConfigureSecurityTokenValidator(oidcOptions, o);
					ConfigureEvents(oidcOptions, o);
				});

			return builder;
		}

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
#if NETCORE2
					context.ProtocolMessage = new ExtendedOidcMessage(context.ProtocolMessage);
#else
					context.ProtocolMessage.EnsureCspForOidcFormPostBehavior();
#endif

					return onRedirectToIdp != null ? onRedirectToIdp(context) : Task.CompletedTask;
				};

				var onRedirectToIdpSignOut = o.Events.OnRedirectToIdentityProviderForSignOut;

				o.Events.OnRedirectToIdentityProviderForSignOut = context =>
				{
#if NETCORE2
					context.ProtocolMessage = new ExtendedOidcMessage(context.ProtocolMessage);
#else
					context.ProtocolMessage.EnsureCspForOidcFormPostBehavior();
#endif

					return onRedirectToIdpSignOut != null ? onRedirectToIdpSignOut(context) : Task.CompletedTask;
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

			var sessionService = services.FirstOrDefault(s => s.ServiceType.Name == nameof(ISessionStore));

			if (sessionService == null) services.AddSession(o => o.Cookie.IsEssential = true);
			else services.Configure<SessionOptions>(o => o.Cookie.IsEssential = true);

			services.AddSession();
			services.AddHttpContextAccessor();
			services.TryAddSingleton<ICacheStorage, SessionCacheStorage>();
			services.SetupTokenCaches(useDataProtection);
			return app;
		}

		/// <summary>
		/// Adds in memory token caches
		/// </summary>
		/// <param name="app"></param>
		/// <param name="cacheConfigAction"></param>
		/// <param name="useDataProtection"></param>
		/// <returns></returns>
		public static AuthenticationBuilder AddInMemoryTokenCaches(this AuthenticationBuilder app, Action<MemoryCacheEntryOptions>? cacheConfigAction = null, bool useDataProtection = true)
		{
			var services = app.Services;
			services.AddMemoryCache();

			if (cacheConfigAction != null) services.Configure(cacheConfigAction);

			services.TryAddSingleton<ICacheStorage, MemoryCacheStorage>();
			services.SetupTokenCaches(useDataProtection);
			return app;
		}

		/// <summary>
		/// Adds distributed token caches
		/// </summary>
		/// <param name="app"></param>
		/// <param name="cacheConfigAction"></param>
		/// <param name="useDataProtection"></param>
		/// <returns></returns>
		public static AuthenticationBuilder AddDistributedTokenCaches(this AuthenticationBuilder app, Action<DistributedCacheEntryOptions>? cacheConfigAction = null, bool useDataProtection = true)
		{
			var services = app.Services;
			services.AddDistributedMemoryCache();

			if (cacheConfigAction != null) services.Configure(cacheConfigAction);

			services.TryAddSingleton<ICacheStorage, DistributedCacheStorage>();
			services.SetupTokenCaches(useDataProtection);
			return app;
		}

		private static void SetupTokenCaches(this IServiceCollection services, bool useDataProtection = true)
		{
			if (useDataProtection) services.AddDataProtection();

			services.TryAddSingleton<ITokenCacheProvider>(p =>
			{
				var tokenCacheProvider = ActivatorUtilities.CreateInstance<TokenCacheProvider>(p);
				tokenCacheProvider.UseDataProtection = useDataProtection;
				return tokenCacheProvider;
			});

			services.TryAddSingleton<IClientAppFactory>(p =>
			{
				var oauth2Options = p.GetRequiredService<OAuth2Options>();
				return ActivatorUtilities.CreateInstance<MsalClientAppFactory>(p, oauth2Options);
			});

			services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, o =>
			{
				var handler = o.Events.OnAuthorizationCodeReceived;

				o.Events.OnAuthorizationCodeReceived = async context =>
				{
					await OnCodeReceived(context).ConfigureAwait(false);
					handler?.Invoke(context).ConfigureAwait(false);
				};
			});

			static async Task OnCodeReceived(AuthorizationCodeReceivedContext context)
			{
				var serviceProvider = context.HttpContext.RequestServices;
				var oauth2Options = serviceProvider.GetRequiredService<OAuth2Options>();

				var codeVerifier = context.TokenEndpointRequest?.GetParameter("code_verifier");
				var authCode = context.TokenEndpointRequest?.Code;
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
