using DNVGL.OAuth.Web.Abstractions;
using DNVGL.OAuth.Web.TokenCache;
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
    /// <summary>
    /// 
    /// </summary>
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
        /// 
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

            builder.AddOpenIdConnect(o =>
            {
                o.ConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(oidcOptions.MetadataAddress, new OpenIdConnectConfigurationRetriever());
                o.Authority = oidcOptions.Authority;
                o.ClientId = oidcOptions.ClientId;
                o.ClientSecret = oidcOptions.ClientSecret;
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
            });

            return builder;
        }
        #endregion

        #region AddDistributedTokenCache
        /// <summary>
        /// Setups distributed cache for MSAL token to OidcOptions.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="oidcOptions"></param>
        /// <param name="cacheSetupAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddDistributedTokenCache(this IServiceCollection services, OidcOptions oidcOptions, Action<DistributedCacheEntryOptions> cacheSetupAction = null)
        {
            var cacheEntryOptions = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60) };
            cacheSetupAction?.Invoke(cacheEntryOptions);

            services.AddSingleton<ITokenCacheProvider>(f => new MsalTokenCacheProvider(f.GetRequiredService<IDistributedCache>(), cacheEntryOptions));

            oidcOptions.Events = new OpenIdConnectEvents
            {
                OnAuthorizationCodeReceived = async context =>
                {
                    var msalClientApp = context.HttpContext.RequestServices.GetService<IClientAppBuilder>()
                        .WithOpenIdConnectOptions(oidcOptions)
                        .BuildForUserCredentials(context);
                    var result = await msalClientApp.AcquireTokenByAuthorizationCode(context);
                }
            };

            services.AddSingleton(f => new MsalClientAppBuilder(f.GetRequiredService<ITokenCacheProvider>()).WithOpenIdConnectOptions(oidcOptions));
            return services;
        }
        #endregion
    }
}
