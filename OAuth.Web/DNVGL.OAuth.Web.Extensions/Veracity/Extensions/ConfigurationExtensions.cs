using System;
using System.Threading.Tasks;
using DNV.OAuth.Web.Extensions.Veracity.Constants;
using DNV.OAuth.Web.Extensions.Veracity.Validator;
using DNVGL.OAuth.Web;
using DNVGL.Veracity.Services.Api.My.Abstractions;
using DNVGL.Veracity.Services.Api.My.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DNV.OAuth.Web.Extensions.Veracity.Extensions
{
	public static class ConfigurationExtensions
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="services"></param>
		/// <param name="oidcSetupAction"></param>
		/// <param name="policyValidationSetupAction"></param>
		/// <param name="cookieSetupAction"></param>
		/// <param name="cacheSetupAction"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static AuthenticationBuilder AddOidcWithPolicyValidation(this IServiceCollection services,
			Action<OidcOptions> oidcSetupAction,
			Action<PolicyValidationOptions> policyValidationSetupAction,
			Action<CookieAuthenticationOptions> cookieSetupAction = null,
			Action<DistributedCacheEntryOptions> cacheSetupAction = null)
		{
			if (oidcSetupAction == null)
			{
				throw new ArgumentNullException(nameof(oidcSetupAction));
			}

			if (policyValidationSetupAction == null)
			{
				throw new ArgumentNullException(nameof(policyValidationSetupAction));
			}

			var oidcOptions = new OidcOptions();
			oidcSetupAction(oidcOptions);

			var policyValidationOptions = new PolicyValidationOptions();
			policyValidationSetupAction(policyValidationOptions);

			return services.AddOidcWithPolicyValidation(oidcOptions, policyValidationOptions, cookieSetupAction,
				cacheSetupAction);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="oidcOptions"></param>
		/// <returns></returns>
		public static AuthenticationBuilder AddOidcWithPolicyValidation(this IServiceCollection services,
			OidcOptions oidcOptions,
			PolicyValidationOptions policyValidationOptions,
			Action<CookieAuthenticationOptions> cookieSetupAction = null,
			Action<DistributedCacheEntryOptions> cacheSetupAction = null)
		{
			oidcOptions.AddPolicyValidation(policyValidationOptions);

			services.AddServices(policyValidationOptions.VeracityPolicyApiConfigName);

			return services.AddOidc(oidcOptions, cookieSetupAction, cacheSetupAction);
		}

		internal const string VeracityDefaultPolicy = nameof(VeracityDefaultPolicy);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="services"></param>
		/// <param name="optionSetupAction"></param>
		/// <returns></returns>
		public static IServiceCollection AddAuthorizationWithPolicyValidation(this IServiceCollection services,
			Action<AuthorizationOptions> optionSetupAction = null)
		{
			return services.AddAuthorization(opt =>
			{
				optionSetupAction?.Invoke(opt);
				opt.AddPolicy(VeracityDefaultPolicy,
					pb => pb.Combine(opt.DefaultPolicy).RequireClaim(TokenClaimTypes.VeracityPolicyValidated));
				opt.DefaultPolicy = opt.GetPolicy(VeracityDefaultPolicy);
			});
		}

		private static IServiceCollection AddServices(this IServiceCollection services, string apiConfigName)
		{
			services.TryAddSingleton<IPolicyValidator, PolicyValidator>();
			services.TryAddSingleton<IPolicyViolationHandler, PolicyViolationHandler>();
			services.AddMyPolicies(apiConfigName);

			return services;
		}

		private static OidcOptions AddPolicyValidation(this OidcOptions oidcOptions,
			PolicyValidationOptions policyValidationOptions)
		{
			if (oidcOptions == null)
				throw new ArgumentNullException(nameof(oidcOptions));

			if (policyValidationOptions == null)
				throw new ArgumentNullException(nameof(policyValidationOptions));

			oidcOptions.Events = oidcOptions.Events ?? new OpenIdConnectEvents();

			var handler = oidcOptions.Events.OnTokenValidated;
			oidcOptions.Events.OnTokenValidated = async ctx =>
			{
				var result = await Validate(ctx, policyValidationOptions).ConfigureAwait(false);

				if (result && handler != null)
					await handler(ctx).ConfigureAwait(false);
			};

			return oidcOptions;
		}

		private static async Task<bool> Validate(TokenValidatedContext ctx, PolicyValidationOptions policyValidationOptions)
		{
			var cu = ctx.HttpContext.User;
			ctx.HttpContext.User = ctx.Principal;

			try
			{
				var validator = ctx.HttpContext.RequestServices.GetRequiredService<IPolicyValidator>();
				return await validator.Validate(ctx, policyValidationOptions);
			}
			catch (Exception e)
			{
				ctx.Fail(e);
				return false;
			}
			finally
			{
				ctx.HttpContext.User = cu;
			}
		}
	}
}