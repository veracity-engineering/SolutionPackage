using DNV.OAuth.Abstractions;
using System.Collections;
using System.Collections.Generic;

namespace DNVGL.OAuth.Api.HttpClient
{
    public class OAuthHttpClientOptions
    {
        /// <summary>
        /// Gets or sets a unique identifier for HTTP client instance which is used to retrieve it from the IOAuthHttpClientFactory.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the default credential flow applied to requests by the HTTP client instance.
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
        public OAuth2Options OAuthClientOptions { get; set; }


        public void Bind(OAuthHttpClientOptions options)
        {
            Flow = options.Flow;
            SubscriptionKey = options.SubscriptionKey;
            BaseUri = options.BaseUri;
            OAuthClientOptions = options.OAuthClientOptions;
            SubscriptionKey = options.SubscriptionKey;
            Name = options.Name;
        }
    }
}
