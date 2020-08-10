using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Identity.Client;
using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using DNVGL.OAuth.Api.HttpClient;

namespace DNVGL.AuthTest.Web
{
    public static class AuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder AddUserCredentialsAuthentication(this AuthenticationBuilder services, Action<UserCredentialsAuthenticationOptions> configureOptions)
        {
            var options = new UserCredentialsAuthenticationOptions();
            configureOptions(options);
            return services.AddOpenIdConnect(o =>
            {
                o.ConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(options.OpenIdConnectEndpoint, new OpenIdConnectConfigurationRetriever());
                o.Authority = options.Authority;
                o.ClientId = options.ClientId;
                o.ClientSecret = options.ClientSecret;
                o.CallbackPath = options.CallbackPath;
                o.ResponseType = OpenIdConnectResponseType.Code;
                foreach (var scope in options.Scopes)
                {
                    o.Scope.Add(scope);
                }
                o.Events = new OpenIdConnectEvents
                {
                    OnAuthorizationCodeReceived = async context =>
                    {
                        var code = context.ProtocolMessage.Code;

                        var clientApplication = ConfidentialClientApplicationBuilder.Create(o.ClientId)
                            .WithB2CAuthority(o.Authority)
                            .WithRedirectUri(CallbackRedirectUri(context, o.CallbackPath))
                            .WithClientSecret(o.ClientSecret)
                            .Build();

                        var cacheManager = context.HttpContext.RequestServices.GetService<IDistributedTokenCacheManager>();
                        cacheManager.SetCacheInstance(clientApplication.UserTokenCache);

                        try
                        {
                            var authResult = await clientApplication.AcquireTokenByAuthorizationCode(o.Scope, code).ExecuteAsync();

                            var accounts = await clientApplication.GetAccountsAsync();
                            var account = authResult.Account;

                            // AccessToken may be relayed as bearer token and made available to APIs
                            context.HandleCodeRedemption(authResult.AccessToken, authResult.IdToken);
                        }
                        catch (Exception)
                        {
                            //TODO: Handle
                            throw;
                        }
                    }
                };
            });
        }

        private static string CallbackRedirectUri(AuthorizationCodeReceivedContext context, string callbackPath)
        {
            var request = context.HttpContext.Request;
            return UriHelper.BuildAbsolute(request.Scheme, request.Host, request.PathBase, callbackPath);
        }
    }

    public class UserCredentialsAuthenticationOptions
    {
        // OAuth
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Tenant { get; set; }
        public string Policy { get; set; }
        public string ResourceId { get; set; }
        public IEnumerable<string> Scopes { get; set; }

        public string OpenIdConnectEndpoint => $"https://login.microsoftonline.com/te/{Tenant}/{Policy}/v2.0/.well-known/openid-configuration";
        public string Authority => $"https://login.microsoftonline.com/tfp/{Tenant}/{Policy}";
        public string B2CAuthority => $"https://login.microsoftonline.com/tfp/{Tenant}/{Policy}";
        //public string Scope => $"https://{Tenant}/{ResourceId}/user_impersonation";
        public string CallbackPath => "/signin-oidc";
    }
}
