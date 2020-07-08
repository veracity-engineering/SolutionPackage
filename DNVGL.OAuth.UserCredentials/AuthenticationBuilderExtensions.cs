using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Identity.Client;
using Microsoft.AspNetCore.Authentication;

namespace DNVGL.OAuth.UserCredentials
{
    public static partial class AuthenticationBuilderExtensions
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
                o.Scope.Add(options.Scope);
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

                        try
                        {
                            var authResult = await clientApplication.AcquireTokenByAuthorizationCode(o.Scope, code).ExecuteAsync();
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
}
