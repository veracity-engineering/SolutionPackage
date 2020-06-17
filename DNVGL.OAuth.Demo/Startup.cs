using DNVGL.OAuth.Common;
using DNVGL.OAuth.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DNVGL.SolutionPackage.Demo
{
	public class Startup
	{
		public IConfiguration Configuration { get; }

		public Startup(IConfiguration configuration)
		{
			this.Configuration = configuration;
		}

		public void ConfigureServices(IServiceCollection services)
		{
			// add authentication for web app
			services.AddOidc(this.Configuration.GetSection("Oidc"))
				// add authentication for web api
				.AddJwt(this.Configuration, "ECOInsightMobileApi", "JanusWeb");

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
