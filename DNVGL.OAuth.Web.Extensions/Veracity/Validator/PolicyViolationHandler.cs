using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace DNV.OAuth.Web.Extensions.Veracity.Validator
{
	class PolicyViolationHandler : IPolicyViolationHandler
	{
		public Task HandleCompanyAffiliationViolated<TOptions>(RemoteAuthenticationContext<TOptions> ctx, string redirectUrl) where TOptions : AuthenticationSchemeOptions
		{
			ctx.HandleResponse();
			ctx.Response.Redirect(redirectUrl);
			return Task.CompletedTask;
		}

		public async Task HandleServiceSubscriptionViolated<TOptions>(RemoteAuthenticationContext<TOptions> ctx) where TOptions : AuthenticationSchemeOptions
		{
			ctx.HandleResponse();
			ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
			await ctx.Response.WriteAsync("Missing service subscription.");
		}

		public Task HandleTermsAndConditionsViolated<TOptions>(RemoteAuthenticationContext<TOptions> ctx, string redirectUrl) where TOptions : AuthenticationSchemeOptions
		{
			ctx.HandleResponse();
			ctx.Response.Redirect(redirectUrl);
			return Task.CompletedTask;
		}
	}
}