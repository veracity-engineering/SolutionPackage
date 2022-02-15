using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace DNV.OAuth.Web.Extensions.Veracity
{
	public interface IPolicyViolationHandler
	{
		Task HandleTermsAndConditionsViolated<TOptions>(RemoteAuthenticationContext<TOptions> ctx, string redirectUrl) where TOptions : AuthenticationSchemeOptions;
		Task HandleServiceSubscriptionViolated<TOptions>(RemoteAuthenticationContext<TOptions> ctx) where TOptions : AuthenticationSchemeOptions;
		Task HandleCompanyAffiliationViolated<TOptions>(RemoteAuthenticationContext<TOptions> ctx, string redirectUrl) where TOptions : AuthenticationSchemeOptions;
	}
}