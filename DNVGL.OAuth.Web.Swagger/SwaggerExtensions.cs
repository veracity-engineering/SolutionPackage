using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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

					o.CustomOperationIds(d =>
					{
						var descr = d.ActionDescriptor as ControllerActionDescriptor;
						return $"{descr.ControllerName}_{descr.ActionName}";
					});

					if (option.AuthencitationRequired)
					{
						var oAuth2SecurityScheme = new OpenApiSecurityScheme
						{
							Type = SecuritySchemeType.OAuth2,
							Flows = new OpenApiOAuthFlows
							{
								Implicit = new OpenApiOAuthFlow
								{
									AuthorizationUrl = new Uri(option.AuthorizationEndpoint),
									Scopes = option.Scopes.ToDictionary(s => s.Scope, s => s.Description)
								},
								AuthorizationCode = new OpenApiOAuthFlow
								{
									AuthorizationUrl = new Uri(option.AuthorizationEndpoint),
									TokenUrl = new Uri(option.TokenEndpoint),
									Scopes = option.Scopes.ToDictionary(s => s.Scope, s => s.Description)
								}
							},
							Reference = new OpenApiReference
							{
								Type = ReferenceType.SecurityScheme,
								Id = "oauth2"
							}
						};

						o.AddSecurityDefinition("oauth2", oAuth2SecurityScheme);

						var httpSecurityScheme = new OpenApiSecurityScheme
						{
							Type = SecuritySchemeType.Http,
							Scheme = "Bearer",
							Reference = new OpenApiReference
							{
								Type = ReferenceType.SecurityScheme,
								Id = "http"
							}
						};

						o.AddSecurityDefinition("http", httpSecurityScheme);

						var securityRequirement = new OpenApiSecurityRequirement { 
							{ oAuth2SecurityScheme, new List<string>() },
							{ httpSecurityScheme, new List<string>() }
						};

						o.AddSecurityRequirement(securityRequirement);
					}
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
					o.OAuthClientSecret(option.ClientSecret);
				});
			}

			return app;
		}
	}
}
