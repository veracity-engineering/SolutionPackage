using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DNVGL.OAuth.UserCredentials
{
    public class OAuthHttpClientFactoryOptions
    {
        public string Name { get; private set; }
        public OAuthCredentialFlow Flow { get; set; }

        // API
        public string BaseUrl { get; set; }
        public string SubscriptionKey { get; set; }

        // OAuth
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Authority { get; set; }
        public IEnumerable<string> Scopes { get; set; }

        public OAuthHttpClientFactoryOptions(string name)
        {
            Name = name;
        }
    }

    public enum OAuthCredentialFlow
    {
        [EnumMember(Value = "user-credentials")]
        UserCredentials,
        [EnumMember(Value = "client-credentials")]
        ClientCredentials
    }
}
