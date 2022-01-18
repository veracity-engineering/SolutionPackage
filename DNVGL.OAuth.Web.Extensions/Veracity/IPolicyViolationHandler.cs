using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace DNVGL.OAuth.Web.Extensions.Veracity
{
	public interface IPolicyViolationHandler
	{
		Task TermsAndConditionsPolicyViolated<TOptions>(RemoteAuthenticationContext<TOptions> context, PolicyValidationOptions options) where TOptions : AuthenticationSchemeOptions;
		Task ServiceSubscriptionPolicyViolated<TOptions>(RemoteAuthenticationContext<TOptions> context, PolicyValidationOptions options) where TOptions : AuthenticationSchemeOptions;
		Task CompanyAffiliationPolicyViolated<TOptions>(RemoteAuthenticationContext<TOptions> context, PolicyValidationOptions options) where TOptions : AuthenticationSchemeOptions;
	}

	class DefaultPolicyViolationHandler : IPolicyViolationHandler
	{
		public Task CompanyAffiliationPolicyViolated<TOptions>(RemoteAuthenticationContext<TOptions> context, PolicyValidationOptions options) where TOptions : AuthenticationSchemeOptions
		{
			throw new System.NotImplementedException();
		}

		public Task ServiceSubscriptionPolicyViolated<TOptions>(RemoteAuthenticationContext<TOptions> context, PolicyValidationOptions options) where TOptions : AuthenticationSchemeOptions
		{
			throw new System.NotImplementedException();
		}

		public Task TermsAndConditionsPolicyViolated<TOptions>(RemoteAuthenticationContext<TOptions> context, PolicyValidationOptions options) where TOptions : AuthenticationSchemeOptions
		{
			throw new System.NotImplementedException();
		}
	}
}