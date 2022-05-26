using System;
using System.Threading.Tasks;
using DNV.OAuth.Web.Extensions.Policy.Constants;
using DNV.OAuth.Web.Extensions.Policy.Validator;
using DNVGL.OAuth.Web;
using DNVGL.Veracity.Services.Api.My.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace DNV.OAuth.Web.Extensions.Policy
{
	/// <summary>
	/// 
	/// </summary>
	public static class PolicyExtensions
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="builder"></param>
		/// <param name="configAction"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static AuthenticationBuilder AddPolicyValidation(this AuthenticationBuilder builder, Action<PolicyValidationOptions> configAction)
		{
			if (configAction == null)
			{
				throw new ArgumentNullException(nameof(configAction));
			}

			var options = new PolicyValidationOptions();
			
			configAction(options);

			return builder.AddPolicyValidation(options);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="builder"></param>
		/// <param name="policyValidationOptions"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static AuthenticationBuilder AddPolicyValidation(this AuthenticationBuilder builder, PolicyValidationOptions policyValidationOptions)
		{
			if (policyValidationOptions == null)
				throw new ArgumentNullException(nameof(policyValidationOptions));
			
			if (string.IsNullOrEmpty(policyValidationOptions.VeracityPolicyApiConfigName))
				throw new ArgumentNullException($"{policyValidationOptions}.{nameof(policyValidationOptions.VeracityPolicyApiConfigName)}");

			if (string.IsNullOrEmpty(policyValidationOptions.AuthorizationPolicyName))
				throw new ArgumentNullException($"{policyValidationOptions}.{nameof(policyValidationOptions.AuthorizationPolicyName)}");

			builder.Services
				.AddDependencies(policyValidationOptions.VeracityPolicyApiConfigName!)
				.AddAuthorizationPolicy(policyValidationOptions.AuthorizationPolicyName, policyValidationOptions.AddAsDefaultPolicy)
				.Configure<OpenIdConnectOptions>(
					OpenIdConnectDefaults.AuthenticationScheme,
					o => o.ConfigPolicyValidation(policyValidationOptions));

			return builder;
		}

		private static IServiceCollection AddDependencies(this IServiceCollection services, string apiConfigName)
		{
			services.TryAddSingleton<IPolicyValidator, PolicyValidator>();
			services.TryAddSingleton<IPolicyViolationHandler, PolicyViolationHandler>();
			services.AddMyPolicies(apiConfigName);

			return services;
		}


		private static IServiceCollection AddAuthorizationPolicy(this IServiceCollection services, string policyName, bool addAsDefault)
		{
			return services.AddAuthorization(opt =>
			{
				opt.AddPolicy(policyName,
					pb =>
					{
						if (addAsDefault)
							pb = pb.Combine(opt.DefaultPolicy);
						pb.RequireClaim(TokenClaimTypes.VeracityPolicyValidated);
					});
				
				if (addAsDefault)
					opt.DefaultPolicy = opt.GetPolicy(policyName);
			});
		}
		
		private static void ConfigPolicyValidation(this OpenIdConnectOptions oidcOptions, PolicyValidationOptions policyValidationOptions)
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