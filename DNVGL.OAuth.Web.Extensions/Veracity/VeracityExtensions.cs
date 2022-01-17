using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DNVGL.OAuth.Api.HttpClient.Extensions;
using DNVGL.Veracity.Services.Api.My.Abstractions;
using DNVGL.Veracity.Services.Api.My.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;

namespace DNVGL.OAuth.Web.Extensions.Veracity
{
	public static class VeracityExtensions
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="options"></param>
		/// <returns></returns>
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

			return services.AddOidcWithPolicyValidation(oidcOptions, policyValidationOptions, cookieSetupAction, cacheSetupAction);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="options"></param>
		/// <returns></returns>
		public static AuthenticationBuilder AddOidcWithPolicyValidation(this IServiceCollection services,
			OidcOptions oidcOptions,
			PolicyValidationOptions policyValidationOptions,
			Action<CookieAuthenticationOptions> cookieSetupAction = null,
			Action<DistributedCacheEntryOptions> cacheSetupAction = null)
		{
			oidcOptions.AddPolicyValidation(policyValidationOptions);
			
			services.AddMyPolicies(policyValidationOptions.MyPoliciesApiConfigName);

			return services.AddOidc(oidcOptions, cookieSetupAction, cacheSetupAction);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="oidcOptions"></param>
		/// <param name="policyValidationOptions"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		private static OidcOptions AddPolicyValidation(this OidcOptions oidcOptions, PolicyValidationOptions policyValidationOptions)
		{
			if (oidcOptions == null)
				throw new ArgumentNullException(nameof(oidcOptions));

			if (policyValidationOptions == null)
				throw new ArgumentNullException(nameof(policyValidationOptions));

			oidcOptions.Events = oidcOptions.Events ?? new OpenIdConnectEvents();

			var previous = oidcOptions.Events.OnTokenValidated;
			oidcOptions.Events.OnTokenValidated = async ctx =>
			{
				if (previous != null)
					await previous(ctx).ConfigureAwait(false);

				await Validate(ctx, policyValidationOptions).ConfigureAwait(false);
			};

			return oidcOptions;
		}
		
		private static async Task Validate(TokenValidatedContext ctx, PolicyValidationOptions policyValidationOptions)
		{
			var cu = ctx.HttpContext.User;
			ctx.HttpContext.User = ctx.Principal;

			var validator = ctx.HttpContext.RequestServices.GetRequiredService<IPolicyValidator>();
			try
			{
				await validator.Validate(ctx);
			}
			catch (Exception e)
			{
				ctx.Fail(e);
			}

			ctx.HttpContext.User = cu;
		}
	}
