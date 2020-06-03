using DNVGL.OAuth.Common;
using DNVGL.OAuth.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DNVGL.SolutionPackage.Demo
{
	public class Startup
	{
		public IConfiguration Configuration { get; }
		public OidcOption OidcOptions { get; set; }

		public Startup(IConfiguration configuration)
		{
			this.Configuration = configuration;
		}

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddOidc(this.Configuration, "ECOInsightMobileApi");

			services.AddControllers();

			services.AddSwagger(this.Configuration);
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseHttpsRedirection().UseRouting();

			app.UseOidc();

			app.UseEndpoints(endpoints => endpoints.MapControllers());

			app.UseSwagger(this.Configuration);
		}

	}
}
