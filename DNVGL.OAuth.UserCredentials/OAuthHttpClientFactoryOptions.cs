using System.Runtime.Serialization;

namespace DNVGL.OAuth.Api.HttpClient
{
    public class OAuthHttpClientFactoryOptions
    {
        public string Name { get; set; }
        public OAuthCredentialFlow Flow { get; set; }

        // API
        public string BaseUri { get; set; }
        public string SubscriptionKey { get; set; }

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
