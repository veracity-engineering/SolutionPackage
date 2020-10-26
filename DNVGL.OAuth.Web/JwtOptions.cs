using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace DNVGL.OAuth.Web
{
	public class JwtOptions
	{
		public string TenantId { get; set; }

		public string ClientId { get; set; }

		public string SignInPolicy { get; set; }

		public string Authority => $"https://login.microsoftonline.com/tfp/{this.TenantId}/{this.SignInPolicy}";

		public string MetadataAddress => $"{this.Authority}/v2.0/.well-known/openid-configuration";

		public JwtBearerEvents Events { get; set; }
	}
}
