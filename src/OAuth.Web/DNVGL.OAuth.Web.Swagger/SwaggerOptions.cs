using System;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DNVGL.OAuth.Web.Swagger
{
	public class SwaggerOptions
	{
		public bool Enabled { get; set; } = false;
		public string Version { get; set; } = "v1";
		public string ClientId { get; set; }
		public string ClientSecret { get; set; }
		public string DocumentTitle { get; set; } = "Swagger UI";
		public OpenApiInfo ApiInfo { get; set; }
		public OpenApiOAuthFlow ImplicitFlow { get; set; }
		public OpenApiOAuthFlow AuthCodeFlow { get; set; }
		public OpenApiOAuthFlow ClientCredsFlow { get; set; }
		public Action<SwaggerGenOptions> PostConfigure { get; set; }
	}
}