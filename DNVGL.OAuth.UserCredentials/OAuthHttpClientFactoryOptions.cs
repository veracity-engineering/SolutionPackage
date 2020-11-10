using System.Runtime.Serialization;

namespace DNVGL.OAuth.Api.HttpClient
{
    public class OAuthHttpClientFactoryOptions
    {
        /// <summary>
        /// A unique identifier for HTTP client instance which is used to retrieve it from the IOAuthHttpClientFactory.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The credential flow applied to requests by the HTTP client instance.
        /// </summary>
        public OAuthCredentialFlow Flow { get; set; }

        // API
        /// <summary>
        /// The route URI (Universal Resource Identifier) for the Web API the HTTP client instance will make requests to.
        /// </summary>
        public string BaseUri { get; set; }

        /// <summary>
        /// The subscription key (from API management) for the Web API the HTTP client instance will connect to.
        /// </summary>
        public string SubscriptionKey { get; set; }

        /// <summary>
        /// The Open Id Connection options to apply to authentication by the HTTP client instance.
        /// </summary>
        public OpenIdConnectOptions OpenIdConnectOptions { get; set; }
    }

    public enum OAuthCredentialFlow
    {
        [EnumMember(Value = "user-credentials")]
        UserCredentials,
        [EnumMember(Value = "client-credentials")]
        ClientCredentials
    }

    public class OpenIdConnectOptions
    {
        public string TenantId { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string CallbackPath { get; set; }

        public string ResponseType { get; set; }

        public string[] Scopes { get; set; }

        public string SignInPolicy { get; set; }

        public string Authority => $"https://login.microsoftonline.com/tfp/{TenantId}/{SignInPolicy}";
    }
}
