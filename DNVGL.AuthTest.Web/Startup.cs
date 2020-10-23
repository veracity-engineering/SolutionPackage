using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DNVGL.OAuth.Api.HttpClient;
using DNVGL.OAuth.Api.HttpClient.Extensions;
using DNVGL.OAuth.Web;
using DNVGL.OAuth.Web.Abstractions;
using System.Collections;
using System.Collections.Generic;

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
            var oidcOptions = this.Configuration.GetSection("Oidc").Get<OidcOptions>();

            /*
            services.AddDistributedRedisCache(o =>
            {
                o.InstanceName = "localhost";
                o.Configuration = "localhost";
            });
            */
            services.AddDistributedMemoryCache();
            services.AddDistributedTokenCache(oidcOptions)
            .AddOidc(o =>
            {
                //o = oidcOptions;
                this.Configuration.Bind("Oidc", o);

                o.Events = new Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectEvents
                {
                    OnAuthorizationCodeReceived = async context =>
                    {
                        var msalAppBuilder = context.HttpContext.RequestServices.GetService<IMsalAppBuilder>();
                        var result = await msalAppBuilder.AcquireTokenByAuthorizationCode(context);
                    }
                };
            });

            /*
            services.AddOAuthHttpClientFactory(o =>
            {
                foreach (var child in this.Configuration.GetSection("OAuthHttpClients").GetChildren())
                {
                    o.Add(child.Get<OAuthHttpClientFactoryOptions>());
                }
            });
            */
            services.AddOAuthHttpClientFactory(this.Configuration.GetSection("OAuthHttpClients").Get<IEnumerable<OAuthHttpClientFactoryOptions>>());

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
