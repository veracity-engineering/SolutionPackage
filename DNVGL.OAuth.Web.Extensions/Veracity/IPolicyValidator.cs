using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DNVGL.OAuth.Web.Extensions.Veracity;
using DNVGL.Veracity.Services.Api.Models;
using DNVGL.Veracity.Services.Api.My.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace DNVGL.OAuth.Web.Extensions.Veracity
{
	public interface IPolicyValidator
	{
		Task Validate<TOptions>(RemoteAuthenticationContext<TOptions> ctx, PolicyValidationOptions options) where TOptions : AuthenticationSchemeOptions;
	}

	internal class PolicyValidator : IPolicyValidator
	{
		private readonly IMyPolicies _policies;

		public PolicyValidator(IMyPolicies policies)
		{
			_policies = policies ?? throw new ArgumentNullException(nameof(policies));
		}

		public async Task Validate<TOptions>(RemoteAuthenticationContext<TOptions> ctx, PolicyValidationOptions options) where TOptions : AuthenticationSchemeOptions
		{
			var returnUrl = options.GetReturnUrl?.Invoke(ctx.HttpContext) ?? GetDefaultReturnUrl(ctx);

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
					if (result.SubCode == 0) // no subscription
					{
						ctx.Fail("Policy enforce url is invalid.");
					}
					else if (result.SubCode == 3) // terms hasn't accepted
					{
						if (string.IsNullOrEmpty(result.Url))
						{
							ctx.Fail("Policy enforce url is invalid.");
							return;
						}

						ctx.Properties.RedirectUri = result.Url;
						ctx.HandleResponse();
					}
					break;
				// case (int)HttpStatusCode.NotFound:
				// 	break;
				default:
					ctx.Fail($"Got unexpected StatusCode '{result.StatusCode}' (SubCode: {result.SubCode}) when validate the policy. +('{result.Message}')");
					return;
			}
		}

		private static string GetDefaultReturnUrl<TOptions>(RemoteAuthenticationContext<TOptions> ctx) where TOptions : AuthenticationSchemeOptions
		{
			return $"{ctx.Request.Scheme}://{ctx.Request.Host}{ctx.Properties.RedirectUri}";
		}
	}
}
