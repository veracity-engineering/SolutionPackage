namespace DNVGL.OAuth.Web.Abstractions
{
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
