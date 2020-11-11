namespace DNVGL.OAuth.Web.Abstractions
{
    public class OpenIdConnectOptions
    {
        
        public string TenantId { get; set; }

        /// <summary>
        /// Gets or sets the 'client_id'.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the 'client_secret'.
        /// </summary>
        public string ClientSecret { get; set; }

        public string CallbackPath { get; set; }

        /// <summary>
        /// Gets or sets the 'response_type'.
        /// </summary>
        public string ResponseType { get; set; }

        /// <summary>
        /// Gets or sets the list of permissions for requests.
        /// </summary>
        public string[] Scopes { get; set; }

        public string SignInPolicy { get; set; }

        public string Authority { get; set; }
    }
}
