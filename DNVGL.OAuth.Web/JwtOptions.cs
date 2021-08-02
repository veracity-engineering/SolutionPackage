using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace DNVGL.OAuth.Web
{
	public class JwtOptions
	{
		public string ClientId { get; set; }

		public string ClientSecret { get; set; }

		public TokenValidationParameters TokenValidationParameters { get; set; }

		public JwtBearerEvents Events { get; set; }

		/// <summary>
		/// Gets or sets the Authority to use when making OpenIdConnect calls.
		/// </summary>
		/// <remarks>
		/// v1
		///     https://login.microsoftonline.com/a68572e3-63ce-4bc1-acdc-b64943502e9d
		///     https://login.veracity.com/tfp/a68572e3-63ce-4bc1-acdc-b64943502e9d/b2c_1a_signinwithadfsidp (don't use it)
		/// v2
		///     https://login.microsoftonline.com/a68572e3-63ce-4bc1-acdc-b64943502e9d/v2.0
		///     https://login.veracity.com/tfp/a68572e3-63ce-4bc1-acdc-b64943502e9d/b2c_1a_signinwithadfsidp/v2.0 (by default)
		/// 
		/// path segment 'tfp' is required for MSAL, it is obsoleted and might be removed in the future.
		/// </remarks>
		public string Authority { get; set; } = "https://login.veracity.com/tfp/a68572e3-63ce-4bc1-acdc-b64943502e9d/b2c_1a_signinwithadfsidp/v2.0";
	}
}
