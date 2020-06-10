namespace DNVGL.AuthTest.Web
{
    public partial class Startup
    {
        public class HardConfig
        {
            public const string ClientId = "6f0bb6fa-e604-43cd-9414-42def1ac7deb"; //"35807f23-80d5-4e97-b07a-21b86013a9ff";
            public const string ClientSecret = "g.i1k-B_63p-oi5U6oQSL5V0DVY2iGZXJ~";
            public const string Tenant = "dnvglb2ctest.onmicrosoft.com";
            public const string AuthPolicy = "B2C_1A_SignInWithADFSIdp";

            public const string AudienceId = "a4a8e726-c1cc-407c-83a0-4ce37f1ce130";

            public static string OpenIdConnectEndpoint => $"https://login.microsoftonline.com/te/{Tenant}/{AuthPolicy}/v2.0/.well-known/openid-configuration";
            public static string Authority => $"https://login.microsoftonline.com/{Tenant}";
            public static string Audience => AudienceId;//$"https://{Tenant}/efb3e529-2f80-458b-aedf-7f4c8c794b45";
            public static string Scope => $"https://{Tenant}/a4a8e726-c1cc-407c-83a0-4ce37f1ce130/user_impersonation";
        }
    }
}
