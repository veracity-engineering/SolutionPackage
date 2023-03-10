using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace DNVGL.OAuth.Web
{
	/// <summary>
	/// 
	/// </summary>
	public class AuthorityItem
	{
		public string? SchemePostfix { get; set; }

		public string? Authority { get; set; }
	}
	
	public class JwtOptions
	{
		/// <summary>
		/// 
		/// </summary>
		public const string JwtDefaultPolicy = nameof(JwtDefaultPolicy);

		/// <summary>
		/// 
		/// </summary>
		public string? ClientId { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public TokenValidationParameters? TokenValidationParameters { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public JwtBearerEvents? Events { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public Func<IEnumerable<Claim>, (bool Succeeded, string FailedReason)>? CustomClaimsValidator { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public ISecurityTokenValidator? SecurityTokenValidator { get; set; }

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
		public string? Authority { get; set; }

		/// <summary>
		/// Multiple Authorities
		/// </summary>
		public List<AuthorityItem> Authorities { get; set; } = new List<AuthorityItem>();

		/// <summary>
		/// 
		/// </summary>
		public string AuthorizationPolicyName { get; set; } = JwtDefaultPolicy;

		/// <summary>
		/// 
		/// </summary>
		public bool AddAsDefault { get; set; } = true;
	}
}
