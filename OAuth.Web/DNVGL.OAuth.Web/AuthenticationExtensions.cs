using DNV.OAuth.Core.TokenCache;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DNV.OAuth.Abstractions;
using DNV.OAuth.Core;
using DNV.OAuth.Core.TokenValidator;

namespace DNVGL.OAuth.Web
{
	public static class AuthenticationExtensions
	{
		private const string DefaultJwtAuthorizationPolicy = nameof(DefaultJwtAuthorizationPolicy);

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

		public static AuthenticationBuilder AddJwt(this AuthenticationBuilder builder, IDictionary<string, JwtOptions> schemaOptions, bool populateAuthorizationPolicy = true)
		{
			if (schemaOptions == null || !schemaOptions.Any())
			{
				throw new ArgumentNullException(nameof(schemaOptions));
			}

			var schemeNames = new List<string>();

			foreach (var schemaOption in schemaOptions)
			{
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
						var schemeName = $"{schemaOption.Key}{aut.SchemePostfix}";
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
			}

			if (populateAuthorizationPolicy && schemeNames.Any())
				builder.Services.AddAuthorization(o =>
				{
					o.AddPolicy(DefaultJwtAuthorizationPolicy,
						p => { p.AddAuthenticationSchemes(schemeNames.ToArray()).RequireAuthenticatedUser(); });
					o.DefaultPolicy = o.GetPolicy(DefaultJwtAuthorizationPolicy);
				});

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
			if (oidcOptions == null)
			{
				throw new ArgumentNullException(nameof(oidcOptions));
			}

			builder = cookieSetupAction != null ? 
				builder.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, 
					o => cookieSetupAction(o)): 
				builder.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

			builder.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, 
				o =>
				{
					o.Authority = oidcOptions.Authority;
					o.ClientId = oidcOptions.ClientId;
					o.ClientSecret = oidcOptions.ClientSecret;
					o.CallbackPath = oidcOptions.CallbackPath;
					o.ResponseType = oidcOptions.ResponseType;
					o.AuthenticationMethod = oidcOptions.AuthenticationMethod;
	#if NETCORE3
					o.UsePkce = true;
	#endif

					ConfigureScopes(oidcOptions, o);
					ConfigureSecurityTokenValidator(oidcOptions, o);
					ConfigureEvents(oidcOptions, o);
				});

			// switch to authorization code flow.
			if (oidcOptions.ResponseType.Split(' ').Contains(OpenIdConnectResponseType.Code))
			{
				AddDistributedTokenCache(builder.Services, oidcOptions, cacheSetupAction);
			}

			return builder;
		}

		private static void ConfigureScopes(OidcOptions oidcOptions, OpenIdConnectOptions o)
		{
			if (oidcOptions.Scopes == null || !oidcOptions.Scopes.Any())
			{
				oidcOptions.Scopes = new string[] { oidcOptions.ClientId };
			}

			foreach (var scope in oidcOptions.Scopes) o.Scope.Add(scope);
		}

		private static void ConfigureSecurityTokenValidator(OidcOptions oidcOptions, OpenIdConnectOptions o)
		{
			o.SecurityTokenValidator = oidcOptions.SecurityTokenValidator ?? new DNVTokenValidator();
		}

		private static void ConfigureEvents(OidcOptions oidcOptions, OpenIdConnectOptions o)
		{
			if (oidcOptions.Events != null) o.Events = oidcOptions.Events;

			if (o.AuthenticationMethod == OpenIdConnectRedirectBehavior.FormPost && o.Events.OnRedirectToIdentityProvider != null)
			{
				var onRedirectToIdentityProvider = o.Events.OnRedirectToIdentityProvider;

				o.Events.OnRedirectToIdentityProvider = context =>
				{
					context.Response.Headers.Remove("content-security-policy");
					return onRedirectToIdentityProvider(context);
				};
			}
		}
		#endregion

		#region AddDistributedTokenCache
		private static IServiceCollection AddDistributedTokenCache(this IServiceCollection services, OidcOptions oidcOptions, Action<DistributedCacheEntryOptions> cacheConfigAction = null)
		{
			services.AddDataProtection();

			services.AddOAuthCore(cacheConfigAction)
				.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, o =>
				{
					var previous = o.Events.OnAuthorizationCodeReceived;

					o.Events.OnAuthorizationCodeReceived = async context =>
					{
						await OnCodeReceived(context);
						await previous(context);
					};
				});

			async Task OnCodeReceived(AuthorizationCodeReceivedContext context)
			{
				var codeVerifier = context.TokenEndpointRequest.GetParameter("code_verifier"); 
				var authCode = context.TokenEndpointRequest.Code;
				var clientAppBuilder = context.HttpContext.RequestServices.GetRequiredService<IClientAppBuilder>();
				var clientApp = clientAppBuilder.Build(oidcOptions);

				context.HandleCodeRedemption();
				var result = await clientApp.AcquireTokenByAuthorizationCode(authCode, codeVerifier);
				context.HandleCodeRedemption(result.AccessToken, result.IdToken);
			}

			return services;
		}
		#endregion
	}
}
