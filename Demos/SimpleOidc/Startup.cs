using DNVGL.OAuth.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SimpleOidc
{
	public class Startup
	{
		public Startup(IConfiguration configuration)		{			Configuration = configuration;		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			var oidcOptions = new OidcOptions
			{
				TenantId = "a68572e3-63ce-4bc1-acdc-b64943502e9d",
				SignInPolicy = "b2c_1a_signinwithadfsidp",
				ClientId = "34598bb3-b07f-4187-a32b-d64ef8f086bc",
				Scopes = new[] { "https://dnvglb2ctest.onmicrosoft.com/a4a8e726-c1cc-407c-83a0-4ce37f1ce130/user_impersonation" },
				CallbackPath = "/signin-oidc"
			};
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
