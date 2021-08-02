using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace DNVGL.OAuth.Web
{
	public class OidcOptions : Abstractions.OpenIdConnectOptions
	{
		public OpenIdConnectRedirectBehavior AuthenticationMethod { get; set; } = OpenIdConnectRedirectBehavior.FormPost;

		public OpenIdConnectEvents Events { get; set; }
	}
}