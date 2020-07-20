using DNVGL.OAuth.Web;
using DNVGL.OAuth.Web.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DNVGL.OAuth.Demo
{
	public class Startup
	{
		public IConfiguration Configuration { get; }

		public Startup(IConfiguration configuration) { this.Configuration = configuration; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddDistributedMemoryCache();

			// add authentication for web app
			services.AddOidc(o => this.Configuration.GetSection("Oidc").Bind(o))
				// add authentication for web api
				.AddJwt(this.Configuration.GetSection("OidcOptions").GetChildren());

			services.AddControllersWithViews();

			// add swagger generation and swagger UI with authentication feature
			services.AddSwagger(this.Configuration);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}/{id?}");
			});

			// provide parameters to swagger UI
			app.UseSwagger(this.Configuration);
		}
	}
}
