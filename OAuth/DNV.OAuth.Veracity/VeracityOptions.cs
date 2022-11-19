using System;
using System.Collections.Generic;

namespace DNV.OAuth.Veracity
{
	public class VeracityOptions
	{
		private static readonly IDictionary<VeracityEnvironment, VeracityOptions> AllOptions;

		public VeracityEnvironment Environment { get; private set; }
		public string TenantId { get; private set; }
		public string TenantName { get; private set; }
		public string AADHost { get; set; } = "login.microsoftonline.com";
		public string B2CHost { get; private set; }
		public string B2CPolicy { get; set; } = "b2c_1a_signinwithadfsidp";

		private string Tenant => string.IsNullOrEmpty(this.TenantId) ? $"{this.TenantName}.onmicrosoft.com" : this.TenantId;
		public string AADTokenUrl => $"https://{this.AADHost}/{this.Tenant}/oauth2/v2.0/token";
		public string B2CAuthorizationUrl => $"https://{this.B2CHost}/{this.Tenant}/{this.B2CPolicy}/oauth2/v2.0/authorize";
		public string B2CTokenUrl => $"https://{this.B2CHost}/{this.Tenant}/{this.B2CPolicy}/oauth2/v2.0/token";

		/// <summary>
		/// Authority uri for Azure AD client flow v1 endpoint
		/// </summary>
		[Obsolete("Please use AADAuthorityV2 if possible")]
		public string AADAuthorityV1 => $"https://{this.AADHost}/{this.Tenant}";

		/// <summary>
		/// Authority uri for Azure AD client flow v2 endpoint
		/// </summary>
		public string AADAuthorityV2 => $"https://{this.AADHost}/{this.Tenant}/v2.0";

		[Obsolete("Don't use it")]
		public string B2CAuthorityV1 => $"https://{this.B2CHost}/{this.Tenant}/{this.B2CPolicy}";

		/// <summary>
		/// Authority uri for Azure AD B2C user flow v2 endpoint
		/// </summary>
		/// <remarks>
		/// path segment 'tfp' is required for MSAL, but it is obsoleted and might be removed in the future.
		/// </remarks>
		public string B2CAuthorityV2 => $"https://{this.B2CHost}/tfp/{this.Tenant}/{this.B2CPolicy}/v2.0";

		static VeracityOptions()
		{
			AllOptions = new Dictionary<VeracityEnvironment, VeracityOptions>
			{
				{
					VeracityEnvironment.Testing,
					new VeracityOptions(
						VeracityEnvironment.Testing,
						"ed815121-cdfa-4097-b524-e2b23cd36eb6",
						"dnvglb2ctest",
						"logintest.veracity.com"
					)
				},
				{
					VeracityEnvironment.Staging,
					new VeracityOptions(
						VeracityEnvironment.Staging,
						"307530a1-6e70-4ef7-8875-daa8f5a664ec",
						"dnvglb2cstag",
						"loginstag.veracity.com"
					)
				},
				{
					VeracityEnvironment.Production,
					new VeracityOptions(
						VeracityEnvironment.Production,
						"a68572e3-63ce-4bc1-acdc-b64943502e9d",
						"dnvglb2cprod",
						"login.veracity.com"
					)
				},
			};
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="environment"></param>
		/// <param name="tenantId"></param>
		/// <param name="tenantName"></param>
		/// <param name="b2cHost"></param>
		private VeracityOptions(VeracityEnvironment environment, string tenantId, string tenantName, string b2cHost)
		{
			this.Environment = environment;
			this.TenantId = tenantId;
			this.TenantName = tenantName;
			this.B2CHost = b2cHost;
		}

		public string GetAADScope(string scope) => Guid.TryParse(scope, out var guid) ? $"https://{this.TenantName}.onmicrosoft.com/{guid:D}/.default" : scope;

		public string GetB2CScope(string scope) => Guid.TryParse(scope, out var guid) ? $"https://{this.TenantName}.onmicrosoft.com/{guid:D}/user_impersonation" : scope;

		/// <summary>
		/// Creates <see cref="VeracityOptions"/> by giving <see cref="VeracityEnvironment"/>
		/// </summary>
		/// <param name="environment"></param>
		/// <returns></returns>
		public static VeracityOptions Get(VeracityEnvironment environment) => AllOptions[environment];
	}
}
