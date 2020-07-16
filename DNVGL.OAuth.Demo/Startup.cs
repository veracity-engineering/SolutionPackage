using DNVGL.OAuth.Web;
using DNVGL.OAuth.Web.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DNVGL.SolutionPackage.Demo
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

			services.AddMvc();

			// add swagger generation and swagger UI with authentication feature
			services.AddSwagger(this.Configuration);
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseHttpsRedirection();
			app.UseAuthentication();

			app.UseMvc(routes =>
			{
				routes.MapRoute(name: "default", template: "{controller=Home}/{action=Index}/{id?}");
			});

			// provide parameters to swagger UI
			app.UseSwagger(this.Configuration);
		}
	}
}
