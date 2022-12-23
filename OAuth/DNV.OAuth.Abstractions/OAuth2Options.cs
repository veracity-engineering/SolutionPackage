using System;
using System.Linq;

namespace DNV.OAuth.Abstractions
{
	public class OAuth2Options : VeracityClientOptions
	{
		/// <summary>
		/// Gets or sets the Authority to use when making OpenIdConnect calls.
		/// </summary>
		public string Authority { get; set; } 

		/// <summary>
		/// Gets or sets the resource for v1 only.
		/// </summary>
		[Obsolete]
		public string Resource { get; set; }

        /// <summary>
        /// Obsoleted, just keep for compatible reason, will only take the first element
        /// </summary>
        [Obsolete]
        public string[]? Scopes { get; set; }

        /// <summary>
        /// Gets or sets the scope for v2 only.
        /// </summary>
        public string? Scope { get; set; }

        /// <summary>
		/// The request path within the application's base path where the user-agent will be returned. The middleware will process this request when it arrives.
		/// </summary>
		public string CallbackPath { get; set; } = "/signin-oidc";

		/// <summary>
		/// Initializes <see cref="OAuth2Options"/> by its <see cref="VeracityEnvironment"/>.
		/// </summary>
		public virtual void Initialize()
		{
			this.Authority = this.VeracityOptions.AADAuthorityV2;
#pragma warning disable CS0612
			this.Scope = this.VeracityOptions.GetAADScope(this.Scope ?? Scopes?.FirstOrDefault() ?? this.ClientId);
#pragma warning restore CS0612
		}
	}
}
