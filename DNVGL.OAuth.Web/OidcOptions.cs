using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace DNVGL.OAuth.Web
{
	public class OidcOptions : Abstractions.OpenIdConnectOptions
	{
		public string MetadataAddress => $"{this.Authority}/v2.0/.well-known/openid-configuration";

		public OpenIdConnectRedirectBehavior AuthenticationMethod { get; set; } = OpenIdConnectRedirectBehavior.FormPost;

		public OpenIdConnectEvents Events { get; set; }
	}
}