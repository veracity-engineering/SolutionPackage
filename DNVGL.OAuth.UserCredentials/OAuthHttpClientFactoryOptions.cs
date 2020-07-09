using System.Collections.Generic;

namespace DNVGL.OAuth.UserCredentials
{
    public class OAuthHttpClientFactoryOptions
    {
        public string Name { get; private set; }

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
}
