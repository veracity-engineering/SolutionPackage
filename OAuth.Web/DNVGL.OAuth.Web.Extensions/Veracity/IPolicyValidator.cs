using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace DNV.OAuth.Web.Extensions.Veracity
{
	public interface IPolicyValidator
	{
		Task<bool> Validate<TOptions>(RemoteAuthenticationContext<TOptions> ctx, PolicyValidationOptions options) where TOptions : AuthenticationSchemeOptions;
	}
}
