using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DNVGL.OAuth.Api.HttpClient.Extensions;
using DNVGL.OAuth.Web;
using DNVGL.OAuth.Web.Abstractions;
using DNVGL.Veracity.Services.Api.Directory.Extensions;
using DNVGL.Veracity.Services.Api.My.Extensions;

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
                this.Configuration.Bind("Oidc", o);
                o.Events = new Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectEvents
                {
                    OnAuthorizationCodeReceived = async context =>
                    {
                        var msalApp = context.HttpContext.RequestServices.GetService<IClientAppBuilder>()
                            .WithOpenIdConnectOptions(o)
                            .BuildForUserCredentials(context);
                        var result = await msalApp.AcquireTokenByAuthorizationCode(context);
                    }
                };
            });

            services.AddOAuthHttpClientFactory(o => this.Configuration.Bind("OAuthHttpClients", o));
            services.AddSingleton<IUserService, UserService>();

            services.AddUserDirectory();
            services.AddMyProfile();
            //services.AddOAuthHttpClientFactory(this.Configuration.GetSection("OAuthHttpClients").Get<IEnumerable<OAuthHttpClientFactoryOptions>>());

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
