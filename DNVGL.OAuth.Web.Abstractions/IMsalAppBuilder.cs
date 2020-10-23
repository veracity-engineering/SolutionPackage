using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Client;

namespace DNVGL.OAuth.Web.Abstractions
{
    public interface IMsalAppBuilder
    {
        Task<AuthenticationResult> AcquireTokenByAuthorizationCode<TOptions>(RemoteAuthenticationContext<TOptions> context) where TOptions : AuthenticationSchemeOptions;

        Task<AuthenticationResult> AcquireTokenSilent(HttpContext httpContext, string[] scopes);

        Task<IAccount> GetAccount(HttpContext httpContext);

        Task ClearUserTokenCache(HttpContext httpContext);
    }
}
