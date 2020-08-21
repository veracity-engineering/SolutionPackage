using DNVGL.OAuth.Demo.TokenCache;
using DNVGL.OAuth.Web;
using DNVGL.OAuth.Web.Swagger;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
#if NETCORE3
using Microsoft.Extensions.Hosting;
#endif
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DNVGL.OAuth.Demo
{
	public class Startup
	{
		public IConfiguration Configuration { get; }

		public Startup(IConfiguration configuration) { this.Configuration = configuration; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddDistributedMemoryCache()
				.AddSingleton<IMsalTokenCacheProvider>(f => new MsalMemoryTokenCacheProvider(f.GetRequiredService<IDistributedCache>(), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60) }))
				.AddSingleton(f => new MsalAppBuilder(this.Configuration.GetSection("Oidc").Get<OidcOptions>(), f.GetRequiredService<IMsalTokenCacheProvider>()));

			// add authentication for web app
			services.AddOidc(o =>
			{
				this.Configuration.Bind("Oidc", o);

				o.Events = new OpenIdConnectEvents
				{
					OnAuthorizationCodeReceived = async context =>
					{
						var msalAppBuilder = context.HttpContext.RequestServices.GetService<MsalAppBuilder>();
						var result = await msalAppBuilder.AcquireTokenByAuthorizationCode(context);
					},
					OnTokenValidated = context => {
						var claimType = context.Principal.GetMsalAccountIdClaimType();
						var objectId = context.Principal.GetObjectId();
						var tenantId = o.TenantId;
						var policy = o.SignInPolicy;
						var msalAccountId = $"{objectId}-{policy}.{tenantId}";
						(context.Principal.Identity as ClaimsIdentity).AddClaim(new Claim(claimType, msalAccountId));
						return Task.CompletedTask;
					}
				};
			})
				// add authentication for web api
				.AddJwt(this.Configuration.GetSection("OidcOptions").GetChildren());

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
