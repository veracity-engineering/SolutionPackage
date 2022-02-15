using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using DNV.OAuth.Web.Extensions.Veracity.Constants;
using DNVGL.Veracity.Services.Api.Models;
using DNVGL.Veracity.Services.Api.My.Abstractions;
using Microsoft.AspNetCore.Authentication;

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
			var returnUrl = options.GetReturnUrl?.Invoke(ctx.HttpContext) ?? GetDefaultReturnUrl(ctx);

			PolicyValidationResult result;
			if ((options.PolicyValidationMode & PolicyValidationMode.PlatformAndService) > 0)
			{
				result = await _policies.ValidatePolicy(
					options.ServiceId,
					returnUrl, 
					(options.PolicyValidationMode & PolicyValidationMode.ServiceSubscription) > 0 ? "true": "false");
			}
			else if ((options.PolicyValidationMode & PolicyValidationMode.PlatformTermsAndCondition) > 0)
			{
				result = await _policies.ValidatePolicies(returnUrl);
			}
			else
				throw new NotImplementedException($"Unknown {nameof(options.PolicyValidationMode)}: '{(int)options.PolicyValidationMode}'");

			if (result.StatusCode >= (int)HttpStatusCode.OK && result.StatusCode <= 299)
				((ClaimsIdentity)ctx.Principal.Identity).AddClaim(new Claim(TokenClaimTypes.VeracityPolicyValidated, "true"));
			else if (result.StatusCode == (int) HttpStatusCode.NotAcceptable)
			{
				if (result.SubCode == 0) // no subscription
					await _violationHandler.HandleServiceSubscriptionViolated(ctx);
				else if (result.SubCode == 3) // terms hasn't accepted
					await _violationHandler.HandleTermsAndConditionsViolated(ctx, result.Url);
				else
					await _violationHandler.HandleCompanyAffiliationViolated(ctx, result.Url);
			}
		}

		private static string GetDefaultReturnUrl<TOptions>(RemoteAuthenticationContext<TOptions> ctx) where TOptions : AuthenticationSchemeOptions
		{
			return $"{ctx.Request.Scheme}://{ctx.Request.Host}{ctx.Properties.RedirectUri}";
		}
	}
}