using DNV.OAuth.Abstractions;
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

		/// <summary>
		/// Initializes <see cref="OidcOptions"/> by its <see cref="VeracityEnvironment"/>.
		/// </summary>
		public override void Initialize()
		{
			this.Authority = this.VeracityOptions.B2CAuthorityV2;
			this.Scope = this.VeracityOptions.GetB2CScope(this.Scope ?? this.ClientId);
		}
	}
}
