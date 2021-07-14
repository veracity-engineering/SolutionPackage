using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;

namespace DNVGL.OAuth.Web.Swagger
{
	public static class SwaggerExtensions
	{
		public static IServiceCollection AddSwagger(this IServiceCollection services, Action<SwaggerOption> setupAction)
		{
			var option = new SwaggerOption
			{
				Enabled = true,
				Version = "v1"
			};

			setupAction(option);

			if (option.Enabled)
			{
				services.AddSwaggerGen(o =>
				{
					o.SwaggerDoc(option.Version, new OpenApiInfo { Title = option.Name, Version = option.Version });

					var oauth2Schema = new OpenApiSecurityScheme
					{
						Type = SecuritySchemeType.OAuth2,
						Flows = new OpenApiOAuthFlows
						{
							Implicit = new OpenApiOAuthFlow
							{
								AuthorizationUrl = new Uri(option.AuthorizationEndpoint),
								Scopes = option.Scopes
							}
						}
					};

					var apikeySchema = new OpenApiSecurityScheme
					{
						Name = "Authorization",
						In = ParameterLocation.Header,
						Type = SecuritySchemeType.ApiKey
					};

					o.AddSecurityDefinition("OAuth2", oauth2Schema);
					o.AddSecurityDefinition("Bearer", apikeySchema);

					var securityRequirement = new OpenApiSecurityRequirement
					{
						{ 
							new OpenApiSecurityScheme{ Reference = new OpenApiReference{ Id = "OAuth2", Type = ReferenceType.SecurityScheme } },
							new List<string>() 
						},
						{ 
							new OpenApiSecurityScheme{ Reference = new OpenApiReference{ Id = "Bearer", Type = ReferenceType.SecurityScheme } },
							new List<string>() 
						}
					};

					o.AddSecurityRequirement(securityRequirement);
				});
			}

			return services;
		}

		public static IApplicationBuilder UseSwaggerWithUI(this IApplicationBuilder app, Action<SwaggerOption> setupAction)
		{
			var option = new SwaggerOption
			{
				Enabled = true,
				Version = "v1"
			};

			setupAction(option);

			if (option.Enabled)
			{
				app.UseSwagger();

				app.UseSwaggerUI(o =>
				{
					o.SwaggerEndpoint($"{option.Version}/swagger.json", $"{option.Name} {option.Version}");
					o.DisplayRequestDuration();
					o.OAuthAppName($"{option.Name} {option.Version}");
					o.OAuthClientId(option.ClientId);
				}).UseReDoc(o =>
				{
					o.RoutePrefix = "redoc";
					o.SpecUrl = $"/swagger/{option.Version}/swagger.json";
				});
			}

			return app;
		}
	}
}
