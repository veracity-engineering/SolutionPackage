using DNVGL.OAuth.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace TokenCacheDemo
{
	public class Startup
	{
		public Startup(IConfiguration configuration) { Configuration = configuration; }

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			var oidcOptions = new OidcOptions
			{
				TenantId = "ed815121-cdfa-4097-b524-e2b23cd36eb6",
				Authority= "https://login.microsoftonline.com/tfp/ed815121-cdfa-4097-b524-e2b23cd36eb6/B2C_1A_SignInWithADFSIdp",
				SignInPolicy = "b2c_1a_signinwithadfsidp",
				ClientId = "35807f23-80d5-4e97-b07a-21b86013a9ff",
				ClientSecret = "44-TyAb|e:0b^HaL.DlQ)&|6",
				Scopes = new[] { "https://dnvglb2ctest.onmicrosoft.com/a4a8e726-c1cc-407c-83a0-4ce37f1ce130/user_impersonation", "offline_access" },
				CallbackPath = "/signin-oidc",
				ResponseType = OpenIdConnectResponseType.Code
			};
			services.AddDistributedMemoryCache();
			services.AddDistributedTokenCache(oidcOptions);
			services.AddOidc(oidcOptions);

			services.AddControllersWithViews();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseHttpsRedirection().UseRouting();
			app.UseAuthentication().UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}
