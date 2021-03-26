using DNVGL.OAuth.Web.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Identity.Client;
using System;

namespace DNVGL.OAuth.Web.TokenCache
{
    public class MsalClientAppBuilder : IClientAppBuilder
    {
        private readonly ITokenCacheProvider _tokenCacheProvider;
        private Abstractions.OpenIdConnectOptions _options;

        public MsalClientAppBuilder(ITokenCacheProvider tokenCacheProvider)
        {
            _tokenCacheProvider = tokenCacheProvider;
        }

        public IClientAppBuilder WithOpenIdConnectOptions(Abstractions.OpenIdConnectOptions options)
        {
            _options = options;
            return this;
        }

        public IClientApp BuildForClientCredentials()
        {
            var builder = ConfidentialClientApplicationBuilder.Create(_options.ClientId)
                .WithAuthority(new Uri(_options.Authority));

            if (!string.IsNullOrWhiteSpace(_options.ClientSecret))
                builder.WithClientSecret(_options.ClientSecret);

            var clientApp = builder.Build();

            MountCache(clientApp);

            return new MsalClientApp(clientApp, _options);
        }

        public IClientApp BuildForUserCredentials(HttpContext httpContext, string codeVerifier)
        {
            var request = httpContext.Request;
            var returnUri = UriHelper.BuildAbsolute(request.Scheme, request.Host, request.PathBase, _options.CallbackPath);
            var builder = ConfidentialClientApplicationBuilder.Create(_options.ClientId)
                .WithAuthority(new Uri(_options.Authority))
                .WithRedirectUri(returnUri);

            if (!string.IsNullOrWhiteSpace(_options.ClientSecret))
                builder.WithClientSecret(_options.ClientSecret);

            if (!string.IsNullOrWhiteSpace(codeVerifier))
                builder.WithExtraQueryParameters($"code_verifier={codeVerifier}");

            var clientApp = builder.Build();

            MountCache(clientApp);

            return new MsalClientApp(clientApp, _options);
        }

        public IClientApp BuildForUserCredentials<TOptions>(RemoteAuthenticationContext<TOptions> context) where TOptions : AuthenticationSchemeOptions
        {
            var authContext = context as AuthorizationCodeReceivedContext;
            return BuildForUserCredentials(context.HttpContext, authContext.TokenEndpointRequest.GetParameter("code_verifier"));
        }

        private void MountCache(IConfidentialClientApplication confidentialClientApplication)
        {
            if (_tokenCacheProvider != null)
            {
                _tokenCacheProvider.InitializeAsync(confidentialClientApplication.UserTokenCache);
                _tokenCacheProvider.InitializeAsync(confidentialClientApplication.AppTokenCache);
            }
        }
    }
}
