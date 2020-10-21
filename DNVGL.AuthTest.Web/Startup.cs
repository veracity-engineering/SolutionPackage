﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DNVGL.OAuth.Api.HttpClient;
using DNVGL.OAuth.Api.HttpClient.Extensions;
using DNVGL.OAuth.Web;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using DNVGL.OAuth.Web.Abstractions;

namespace DNVGL.AuthTest.Web
{
    public partial class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var oidcOptions = new OidcOptions
            {
                TenantId = "dnvglb2ctest.onmicrosoft.com",
                ClientId = "6f0bb6fa-e604-43cd-9414-42def1ac7deb", // Marketplace client id
                ClientSecret = "g.i1k-B_63p-oi5U6oQSL5V0DVY2iGZXJ~", // Marketplace secret
                CallbackPath = "/signin-oidc",
                Scopes = new[] { "https://dnvglb2ctest.onmicrosoft.com/a4a8e726-c1cc-407c-83a0-4ce37f1ce130/user_impersonation" },
                SignInPolicy = "B2C_1A_SignInWithADFSIdp"
            };

            services.AddDistributedRedisCache(o =>
            {
                o.InstanceName = "localhost";
                o.Configuration = "localhost";
            });
            //services.AddDistributedMemoryCache();
            services.AddDistributedTokenCache(oidcOptions)
            .AddOidc(o =>
            {
                o.ResponseType = OpenIdConnectResponseType.Code;
                o.TenantId = oidcOptions.TenantId;
                o.ClientId = oidcOptions.ClientId;
                o.ClientSecret = oidcOptions.ClientSecret;
                o.CallbackPath = oidcOptions.CallbackPath;
                o.Scopes = oidcOptions.Scopes;
                o.SignInPolicy = oidcOptions.SignInPolicy;
                o.Events = new Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectEvents
                {
                    OnAuthorizationCodeReceived = async context =>
                    {
                        var msalAppBuilder = context.HttpContext.RequestServices.GetService<IMsalAppBuilder>();
                        var result = await msalAppBuilder.AcquireTokenByAuthorizationCode(context);
                    }
                };
            });

            services.AddOAuthHttpClientFactory(o =>
            {
                o.Add(new OAuthHttpClientFactoryOptions("identity-api")
                {
                    Flow = OAuthCredentialFlow.UserCredentials,
                    BaseUri = "https://api-test.veracity.com",
                    SubscriptionKey = "81243fa4-5bf8-4974-b77d-37111e1033ea",
                    OpenIdConnectOptions = new OpenIdConnectOptions
                    {
                        TenantId = oidcOptions.TenantId,
                        ClientId = oidcOptions.ClientId,
                        ClientSecret = oidcOptions.ClientSecret,
                        CallbackPath = oidcOptions.CallbackPath,
                        Scopes = oidcOptions.Scopes,
                        SignInPolicy = oidcOptions.SignInPolicy
                    }
                });
            });

            services.AddMvc(o => o.EnableEndpointRouting = false);//.SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            // app.UseStaticFiles();
            // app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
