using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace DNVGL.OAuth.Web
{
	public class OidcOptions
	{
		public string TenantId { get; set; }

		public string ClientId { get; set; }

		public string ClientSecret { get; set; }

		public string CallbackPath { get; set; }

		public string ResponseType { get; set; }

		public string[] Scopes { get; set; }

		public string SignInPolicy { get; set; }

		public string Authority => $"https://login.microsoftonline.com/tfp/{this.TenantId}/{this.SignInPolicy}";

		public string MetadataAddress => $"{this.Authority}/v2.0/.well-known/openid-configuration";

		public OpenIdConnectEvents Events { get; set; }
	}
}
