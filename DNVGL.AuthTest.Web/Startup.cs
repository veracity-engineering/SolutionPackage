using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Identity.Client;

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
            services.AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                //options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                //options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(o =>
            {
                o.Events = new CookieAuthenticationEvents
                {
                    //OnRedirectToLogin = c => c.HttpContext.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme),
                    OnValidatePrincipal = c => {
                        var p = c.Principal;
                        return Task.CompletedTask;
                    },
                    OnSignedIn = c =>
                    {
                        var p = c.Principal;
                        return Task.CompletedTask;
                    }
                };
            }).AddOpenIdConnect(o =>
            {
                //o.ConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(OpenIdConnectDefaults.AuthenticationScheme, new OpenIdConnectConfigurationRetriever());
                //o.ConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(HardConfig.MetaDataAddress, new OpenIdConnectConfigurationRetriever());
                o.ConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(HardConfig.OpenIdConnectEndpoint, new OpenIdConnectConfigurationRetriever());
                //o.MetadataAddress = HardConfig.OpenIdConnectEndpoint;
                o.Authority = HardConfig.Authority;
                o.ClientId = HardConfig.ClientId;
                o.ClientSecret = HardConfig.ClientSecret;
                o.CallbackPath = HardConfig.CallbackPath;
                o.ResponseType = "code";
                o.Scope.Add("https://dnvglb2ctest.onmicrosoft.com/a4a8e726-c1cc-407c-83a0-4ce37f1ce130/user_impersonation");
                o.Events = new OpenIdConnectEvents()
                {

                    OnAuthorizationCodeReceived = async context =>
                    {
                        var code = context.ProtocolMessage.Code;
                        var request = context.HttpContext.Request;
                        string currentUri = UriHelper.BuildAbsolute(
                            request.Scheme,
                            request.Host,
                            request.PathBase,
                            o.CallbackPath);

                        IConfidentialClientApplication cca = ConfidentialClientApplicationBuilder.Create(o.ClientId)
                            .WithB2CAuthority($"https://login.microsoftonline.com/tfp/{HardConfig.Tenant}/{HardConfig.AuthPolicy}")
                            .WithRedirectUri(currentUri)
                            .WithClientSecret(o.ClientSecret)
                            .Build();
                        
                        try
                        {
                            AuthenticationResult result = await cca.AcquireTokenByAuthorizationCode(o.Scope, code)
                                .ExecuteAsync();
                            context.HandleCodeRedemption(result.AccessToken, result.IdToken);
                        }
                        catch (Exception ex)
                        {
                            //TODO: Handle
                            throw;
                        }
                    }
                };
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
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
