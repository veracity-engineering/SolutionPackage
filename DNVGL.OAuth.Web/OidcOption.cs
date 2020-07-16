using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace DNVGL.OAuth.Web
{
	public class OidcOption
	{
		public string Authority { get; set; }

		public string SignInPolicy { get; set; }

		public string[] Scopes { get; set; }

		public string ClientId { get; set; }

		public string ClientSecret { get; set; }

		public string CallbackPath { get; set; }

		public string ResponseType { get; set; }

		public string MetadataAddress => $"{this.Authority}.well-known/openid-configuration?p={this.SignInPolicy}";

		public OpenIdConnectEvents Events { get; set; }
	}
}
