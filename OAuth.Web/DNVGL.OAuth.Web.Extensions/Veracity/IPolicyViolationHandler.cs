using System.Threading.Tasks;
using DNVGL.Veracity.Services.Api.Models;
using Microsoft.AspNetCore.Authentication;

namespace DNV.OAuth.Web.Extensions.Veracity
{
	public interface IPolicyViolationHandler
	{
		Task HandleTermsAndConditionsViolated<TOptions>(RemoteAuthenticationContext<TOptions> ctx, PolicyValidationResult validationResult) where TOptions : AuthenticationSchemeOptions;
		Task HandleServiceSubscriptionViolated<TOptions>(RemoteAuthenticationContext<TOptions> ctx, PolicyValidationResult validationResult) where TOptions : AuthenticationSchemeOptions;
		Task HandleCompanyAffiliationViolated<TOptions>(RemoteAuthenticationContext<TOptions> ctx, PolicyValidationResult validationResult) where TOptions : AuthenticationSchemeOptions;
	}
}