using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DNVGL.OAuth.Swagger
{
	public static class SwaggerExtensions
	{
		public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration configuration)
		{
			var config = configuration.GetSection("SwaggerOptions");

			if (config == null)
			{
				throw new ArgumentNullException("Cannot find SwaggerOptions in appsettings.json");
			}

			var options = config.Get<SwaggerOption>();

			if (options.Enabled)
			{
				services.AddSwaggerGen(c =>
				{
					c.SwaggerDoc(options.Version, new OpenApiInfo { Title = options.Name, Version = options.Version });

					//c.CustomOperationIds(s =>
					//{
					//	var controller = s.ActionDescriptor.RouteValues["controller"];
					//	var action = s.ActionDescriptor.RouteValues["action"];
					//	var method = s.HttpMethod;
					//	return $"{controller}_{action}_{method}";
					//});

					//var ss = new OpenApiSecurityScheme
					//{
					//	Name = "Veracity Identity Service",
					//	Type = SecuritySchemeType.OAuth2,
					//	Flows = new OpenApiOAuthFlows
					//	{
					//		AuthorizationCode = new OpenApiOAuthFlow
					//		{
					//			Scopes = new Dictionary<string, string> { { oidcOptions.ClientId, null } },
					//			AuthorizationUrl = new Uri(oidcOptions.MetadataAddress + "/oauth2/v2.0/authorize"),
					//			TokenUrl = new Uri("../auth/swaggerToken", UriKind.Relative)
					//		}
					//	}
					//};

					var ss = new OpenApiSecurityScheme
					{
						Type = SecuritySchemeType.OAuth2,
						Flows = new OpenApiOAuthFlows
						{
							Implicit = new OpenApiOAuthFlow
							{
								AuthorizationUrl = new Uri(options.AuthorizationUrl),
								Scopes = options.Scopes.ToDictionary(k => k)
							}
						}
					};

					var sr = new OpenApiSecurityRequirement();
					sr.Add(new OpenApiSecurityScheme
					{
						Reference = new OpenApiReference
						{
							Type = ReferenceType.SecurityScheme,
							Id = "oauth2"
						}
					}, new List<string>());

					c.AddSecurityDefinition("oauth2", ss);
					c.AddSecurityRequirement(sr);
				});
			}

			return services;
		}

		public static IApplicationBuilder UseSwagger(this IApplicationBuilder app, IConfiguration configuration)
		{
			var config = configuration.GetSection("SwaggerOptions");

			if (config == null)
			{
				throw new ArgumentNullException("Cannot find SwaggerOptions in appsettings.json");
			}

			var options = config.Get<SwaggerOption>();

			if (options.Enabled)
			{
				app.UseSwagger();

				app.UseSwaggerUI(c =>
				{
					c.SwaggerEndpoint($"{options.Version}/swagger.json", $"{options.Name} {options.Version}");
					c.DisplayRequestDuration();
					c.OAuthAppName($"{options.Name} {options.Version}");
					c.OAuthClientId(options.ClientId);
				});
			}

			return app;
		}
	}
}
