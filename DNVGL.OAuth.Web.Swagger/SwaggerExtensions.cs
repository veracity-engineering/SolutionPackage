﻿using Microsoft.AspNetCore.Builder;
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

					var securityScheme = new OpenApiSecurityScheme
					{
						Type = SecuritySchemeType.OAuth2,
						Flows = new OpenApiOAuthFlows
						{
							Implicit = new OpenApiOAuthFlow
							{
								AuthorizationUrl = new Uri(option.AuthorizationUrl),
								Scopes = option.Scopes
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
