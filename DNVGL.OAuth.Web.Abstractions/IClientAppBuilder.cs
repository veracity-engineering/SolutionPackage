using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace DNVGL.OAuth.Web.Abstractions
{
	public interface IClientAppBuilder
	{
		IClientAppBuilder WithOAuth2Options(OAuth2Options options);

		IClientApp BuildForUserCredentials(HttpContext httpContext, string codeVerifier = null);

		IClientApp BuildForUserCredentials<TOptions>(RemoteAuthenticationContext<TOptions> context) where TOptions : AuthenticationSchemeOptions;

		IClientApp BuildForClientCredentials();
	}
}
