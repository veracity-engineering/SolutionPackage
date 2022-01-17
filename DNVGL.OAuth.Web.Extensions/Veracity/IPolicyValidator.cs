using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DNVGL.OAuth.Web.Extensions.Veracity;
using DNVGL.Veracity.Services.Api.Models;
using DNVGL.Veracity.Services.Api.My.Abstractions;
using Microsoft.AspNetCore.Authentication;

namespace DNVGL.OAuth.Web.Extensions.Veracity
{
	public interface IPolicyValidator
	{
		Task Validate<TOptions>(RemoteAuthenticationContext<TOptions> context, PolicyValidationOptions options) where TOptions : AuthenticationSchemeOptions;
	}

	internal class PolicyValidator : IPolicyValidator
	{
		private readonly IMyPolicies _policies;

		public PolicyValidator(IMyPolicies policies)
		{
			_policies = policies ?? throw new ArgumentNullException(nameof(policies));
		}

		public async Task Validate<TOptions>(RemoteAuthenticationContext<TOptions> context, PolicyValidationOptions options, string returnUrl) where TOptions : AuthenticationSchemeOptions
		{
			PolicyValidationResult result;
			if ((options.PolicyValidationMode | PolicyValidationMode.PlatformAndService) > 0)
			{
				result = await _policies.ValidatePolicy(
					options.ServiceId, 
					returnUrl, 
					(options.PolicyValidationMode | PolicyValidationMode.ServiceSubscription) > 0 ? "true": "false");
			}
			else if ((options.PolicyValidationMode | PolicyValidationMode.PlatformTermsAndCondition) > 0)
			{
				result = await _policies.ValidatePolicies(returnUrl);
			}
			else
				throw new NotImplementedException($"Unknown {nameof(options.PolicyValidationMode)}: '{(int)options.PolicyValidationMode}'");

			if (result.StatusCode >= (int)HttpStatusCode.OK && result.StatusCode <= 299)
				return;

			switch (result.StatusCode)
			{
				case (int)HttpStatusCode.NotAcceptable:
					break;
				case (int)HttpStatusCode.NotFound:
					break;
				default:
					context.Fail($"Got unexpected StatusCode '{result.StatusCode}' (SubCode: {result.SubCode}) when validate the policy. +('{result.Message}')");
					return;
			}
		}
	}
}
