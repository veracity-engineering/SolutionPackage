using DNVGL.OAuth.Web.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace DNVGL.OAuth.Api.HttpClient
{
    public class OAuthHttpClientFactoryOptions
    {
        /// <summary>
        /// Gets or sets a unique identifier for HTTP client instance which is used to retrieve it from the IOAuthHttpClientFactory.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the credential flow applied to requests by the HTTP client instance.
        /// </summary>
        public OAuthCredentialFlow Flow { get; set; }

        // API
        /// <summary>
        /// Gets or sets the route URI (Universal Resource Identifier) for the Web API the HTTP client instance will make requests to.
        /// </summary>
        public string BaseUri { get; set; }

        /// <summary>
        /// Gets or sets the subscription key (from API management) for the Web API the HTTP client instance will connect to.
        /// </summary>
        public string SubscriptionKey { get; set; }

        /// <summary>
        /// Gets or sets the Open Id Connect options to apply to authentication by the HTTP client instance.
        /// </summary>
        public OAuth2Options OAuth2Options { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum OAuthCredentialFlow
	{
        [EnumMember(Value = "user-credentials")]
        UserCredentials,
        [EnumMember(Value = "client-credentials")]
        ClientCredentials
    }
}
