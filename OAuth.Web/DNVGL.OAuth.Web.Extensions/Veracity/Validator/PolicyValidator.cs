using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using DNV.OAuth.Web.Extensions.Veracity.Constants;
using DNVGL.Veracity.Services.Api.Models;
using DNVGL.Veracity.Services.Api.My.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace DNV.OAuth.Web.Extensions.Veracity.Validator
{
	internal class PolicyValidator : IPolicyValidator
	{
		private readonly IMyPolicies _policies;
		private readonly IPolicyViolationHandler _violationHandler;

		public PolicyValidator(IMyPolicies policies, IPolicyViolationHandler violationHandler)
		{
			_policies = policies ?? throw new ArgumentNullException(nameof(policies));
			_violationHandler = violationHandler ?? throw new ArgumentNullException(nameof(violationHandler));
		}

		public async Task Validate<TOptions>(RemoteAuthenticationContext<TOptions> ctx, PolicyValidationOptions options) where TOptions : AuthenticationSchemeOptions
		{
			var returnUrl = options.GetReturnUrl?.Invoke(ctx.HttpContext, ctx.Properties.RedirectUri) ?? GetDefaultReturnUrl(ctx);

			PolicyValidationResult result;
			if ((options.PolicyValidationMode & PolicyValidationMode.PlatformAndService) > 0)
			{
				result = await _policies.ValidatePolicy(
					options.ServiceId,
					returnUrl, 
					(options.PolicyValidationMode & PolicyValidationMode.ServiceSubscription) != 0 ? "false": "true");
			}
			else if ((options.PolicyValidationMode & PolicyValidationMode.PlatformTermsAndCondition) > 0)
			{
				result = await _policies.ValidatePolicies(returnUrl);
			}
			else
			{
				ctx.Fail($"Invalid {nameof(options.PolicyValidationMode)}: '{(int)options.PolicyValidationMode}', either '{nameof(PolicyValidationMode.PlatformTermsAndCondition)}' or '{nameof(PolicyValidationMode.PlatformTermsAndCondition)}' must be selected.");
				return;
			}

			if (result.StatusCode is >= StatusCodes.Status200OK and <= 299)
			{
				((ClaimsIdentity)ctx.Principal.Identity).AddClaim(new Claim(TokenClaimTypes.VeracityPolicyValidated,
					"true"));
				return;
			}

			if (result.StatusCode is StatusCodes.Status406NotAcceptable or 0)
			{
				// really don't know how to differentiate cases here, always got inconsistent result
				// have to try check result.Url to decide violation type for now

				if (!string.IsNullOrEmpty(result.Url))
				{
					switch (result.SubCode)
					{
						// no subscription
						case 0:
						case 1:
							await _violationHandler.HandleServiceSubscriptionViolated(ctx, result);
							return;
						// terms hasn't accepted
						case 3:
							await _violationHandler.HandleTermsAndConditionsViolated(ctx, result);
							return;
						// no company affiliated with
						default:
							await _violationHandler.HandleCompanyAffiliationViolated(ctx, result);
							return;
					}
				}
			}

			ctx.HandleResponse();
			ctx.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
			await ctx.Response.WriteAsync($"{result.Message}. +(violated: {result.ViolatedPolicies.Aggregate((a, b)=> $"{a}, {b}")})");
		}

		private static string GetDefaultReturnUrl<TOptions>(RemoteAuthenticationContext<TOptions> ctx) where TOptions : AuthenticationSchemeOptions
		{
			return $"{ctx.Request.Scheme}://{ctx.Request.Host}{ctx.Properties.RedirectUri}";
		}
	}
}