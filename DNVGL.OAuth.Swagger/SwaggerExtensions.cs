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
		public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration configuration, string swaggerOptionName = "SwaggerOption")
		{
			var config = configuration.GetSection(swaggerOptionName);

			if (config == null)
			{
				throw new ArgumentNullException("Cannot find SwaggerOption in appsettings.json");
			}

			var option = config.Get<SwaggerOption>();

			if (option.Enabled)
			{
				services.AddSwaggerGen(o =>
				{
					o.SwaggerDoc(option.Version, new OpenApiInfo { Title = option.Name, Version = option.Version });

					var securityScheme = new OpenApiSecurityScheme
					{
						Type = SecuritySchemeType.OAuth2,
						Flows = new OpenApiOAuthFlows
						{
							Implicit = new OpenApiOAuthFlow
							{
								AuthorizationUrl = new Uri(option.AuthorizationUrl),
								Scopes = option.Scopes.ToDictionary(k => k)
							}
						}
					};

					o.AddSecurityDefinition("oauth2", securityScheme);

					var securityRequirement = new OpenApiSecurityRequirement();
					securityRequirement.Add(new OpenApiSecurityScheme
					{
						Reference = new OpenApiReference
						{
							Type = ReferenceType.SecurityScheme,
							Id = "oauth2"
						}
					}, new List<string>());

					o.AddSecurityRequirement(securityRequirement);
				});
			}

			return services;
		}

		public static IApplicationBuilder UseSwagger(this IApplicationBuilder app, IConfiguration configuration, string swaggerOptionName = "SwaggerOption")
		{
			var config = configuration.GetSection(swaggerOptionName);

			if (config == null)
			{
				throw new ArgumentNullException("Cannot find SwaggerOption in appsettings.json");
			}

			var option = config.Get<SwaggerOption>();

			if (option.Enabled)
			{
				app.UseSwagger();

				app.UseSwaggerUI(o =>
				{
					o.SwaggerEndpoint($"{option.Version}/swagger.json", $"{option.Name} {option.Version}");
					o.DisplayRequestDuration();
					o.OAuthAppName($"{option.Name} {option.Version}");
					o.OAuthClientId(option.ClientId);
				});
			}

			return app;
		}
	}
}
