namespace DNVGL.OAuth.UserCredentials
{
    public static partial class AuthenticationBuilderExtensions
    {
        public class UserCredentialsAuthenticationOptions
        {
            public string ClientId { get; set; }
            public string ClientSecret { get; set; }
            public string Tenant { get; set; }
            public string Policy { get; set; }
            public string ResourceId { get; set; }
            //public OpenIdConnectEvents Events { get; set; }

            public string OpenIdConnectEndpoint => $"https://login.microsoftonline.com/te/{Tenant}/{Policy}/v2.0/.well-known/openid-configuration";
            public string Authority => $"https://login.microsoftonline.com/tfp/{Tenant}/{Policy}";
            public string B2CAuthority => $"https://login.microsoftonline.com/tfp/{Tenant}/{Policy}";
            public string Scope => $"https://{Tenant}/{ResourceId}/user_impersonation";
            public string CallbackPath => "/signin-oidc";
        }
    }
}
