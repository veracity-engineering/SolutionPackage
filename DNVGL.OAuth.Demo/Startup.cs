using DNVGL.OAuth.Web;
using DNVGL.OAuth.Web.Abstractions;
using DNVGL.OAuth.Web.Swagger;
using DNVGL.OAuth.Web.TokenCache;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
#if NETCORE3
using Microsoft.Extensions.Hosting;
#endif

namespace DNVGL.OAuth.Demo
{
	public class Startup
	{
		public IConfiguration Configuration { get; }

		public Startup(IConfiguration configuration) { this.Configuration = configuration; }

		public void ConfigureServices(IServiceCollection services)
		{
			var oidcOptions = this.Configuration.GetSection("Oidc").Get<OidcOptions>();

			// add memory cache
			//services.AddDistributedMemoryCache();

			// add redis cache
			/*
			services.AddDistributedRedisCache(o =>
			{
				o.InstanceName = "localhost";
				o.Configuration = "localhost";
			});
			*/
			// add token cache support
            services.AddDistributedMemoryCache();
			services.AddDistributedTokenCache(oidcOptions);

			// add authentication for web app
			services.AddOidc(o =>
			{
				this.Configuration.Bind("Oidc", o);

				o.Events = new OpenIdConnectEvents
				{
					OnAuthorizationCodeReceived = async context =>
					{
						var msalAppBuilder = context.HttpContext.RequestServices.GetService<IMsalAppBuilder>();
						var result = await msalAppBuilder.AcquireTokenByAuthorizationCode(context);

					}
				};
			}).AddJwt(this.Configuration.GetSection("OidcOptions").GetChildren());

#if NETCORE2
			services.AddMvc();
#elif NETCORE3
			services.AddControllersWithViews();
#endif

			// add swagger generation and swagger UI with authentication feature
			services.AddSwagger(o => this.Configuration.GetSection("SwaggerOption").Bind(o));
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
#if NETCORE2
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
#elif NETCORE3

		public void Configure(IApplicationBuilder app, IHostEnvironment env)
#endif
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

#if NETCORE2
			app.UseHttpsRedirection();
			app.UseAuthentication();
			app.UseMvc(routes =>
			{
				routes.MapRoute(name: "default", template: "{controller=Home}/{action=Index}/{id?}");
			});
#elif NETCORE3
			app.UseHttpsRedirection().UseRouting();
			app.UseAuthentication().UseAuthorization();
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
			});
#endif

			// provide parameters to swagger UI
			app.UseSwaggerWithUI(o => this.Configuration.GetSection("SwaggerOption").Bind(o));
		}
	}
}
