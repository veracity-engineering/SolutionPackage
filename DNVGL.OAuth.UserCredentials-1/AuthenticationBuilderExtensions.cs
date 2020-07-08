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
    public static class AuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder AddUserCredentialsAuthentication(this AuthenticationBuilder services, Action<UserCredentialsAuthenticationOptions> configureOptions)
        {
            var options = new UserCredentialsAuthenticationOptions();
            configureOptions(options);
            return services.AddOpenIdConnect(o =>
            {
                o.ConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(options.OpenIdConnectEndpoint, new OpenIdConnectConfigurationRetriever());
                //o.Authority = HardConfig.Authority;
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
                        var request = context.HttpContext.Request;
                        var currentUri = UriHelper.BuildAbsolute(request.Scheme, request.Host, request.PathBase, o.CallbackPath);

                        var clientApplication = ConfidentialClientApplicationBuilder.Create(o.ClientId)
                            .WithB2CAuthority(options.B2CAuthority)
                            .WithRedirectUri(currentUri)
                            .WithClientSecret(o.ClientSecret)
                            .Build();

                        try
                        {
                            var authResult = await clientApplication.AcquireTokenByAuthorizationCode(o.Scope, code).ExecuteAsync();
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

        public class UserCredentialsAuthenticationOptions
        {
            public string ClientId { get; set;}
            public string ClientSecret { get; set; }
            public string Tenant { get; set; }
            public string Policy { get; set; }
            public string ResourceId { get; set; }
            public OpenIdConnectEvents Events { get; set; }

            public string OpenIdConnectEndpoint => $"https://login.microsoftonline.com/te/{Tenant}/{Policy}/v2.0/.well-known/openid-configuration";
            public string B2CAuthority => $"https://login.microsoftonline.com/tfp/{Tenant}/{Policy}";
            public string Scope => $"https://{Tenant}/{ResourceId}/user_impersonation";
            public string CallbackPath => "/signin-oidc";
        }
    }
}
