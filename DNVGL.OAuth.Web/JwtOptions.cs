using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace DNVGL.OAuth.Web
{
	public class JwtOptions
	{
		public string ClientId { get; set; }

		public string Authority { get; set; }

		public string MetadataAddress => $"{this.Authority}/.well-known/openid-configuration";

		public TokenValidationParameters TokenValidationParameters { get; set; }

		public JwtBearerEvents Events { get; set; }
	}
}
