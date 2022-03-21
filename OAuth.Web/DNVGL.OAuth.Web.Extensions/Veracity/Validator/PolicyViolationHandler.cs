using System;
using System.Threading.Tasks;
using DNVGL.Veracity.Services.Api.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace DNV.OAuth.Web.Extensions.Veracity.Validator
{
	class PolicyViolationHandler : IPolicyViolationHandler
	{
		public virtual Task HandleCompanyAffiliationViolated<TOptions>(RemoteAuthenticationContext<TOptions> ctx, PolicyValidationResult validationResult) where TOptions : AuthenticationSchemeOptions
		{
			return HandleRedirect(ctx, validationResult);
		}

		public virtual Task HandleServiceSubscriptionViolated<TOptions>(RemoteAuthenticationContext<TOptions> ctx, PolicyValidationResult validationResult) where TOptions : AuthenticationSchemeOptions
		{
			return HandleRedirect(ctx, validationResult);
		}

		public virtual Task HandleTermsAndConditionsViolated<TOptions>(RemoteAuthenticationContext<TOptions> ctx, PolicyValidationResult validationResult) where TOptions : AuthenticationSchemeOptions
		{
			return HandleRedirect(ctx, validationResult);
		}

		private async Task HandleRedirect<TOptions>(RemoteAuthenticationContext<TOptions> ctx, PolicyValidationResult validationResult) where TOptions : AuthenticationSchemeOptions
		{
			ctx.HandleResponse();

			if (!string.IsNullOrEmpty(validationResult.Url))
				ctx.Response.Redirect(validationResult.Url);
			else if (!string.IsNullOrEmpty(validationResult.Message))
			{
				ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
				await ctx.Response.WriteAsync(validationResult.Message);
			}
		}
	}
}