using DNVGL.OAuth.Web.Abstractions;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace DNVGL.OAuth.Web
{
	public class OidcOptions : OAuth2Options
	{
		/// <summary>
		/// Gets or sets the 'response_type'.
		/// </summary>
		public string ResponseType { get; set; } = "id_token";

		public OpenIdConnectRedirectBehavior AuthenticationMethod { get; set; } = OpenIdConnectRedirectBehavior.FormPost;

		public OpenIdConnectEvents Events { get; set; }

		public ISecurityTokenValidator SecurityTokenValidator { get; set; }
	}
}
