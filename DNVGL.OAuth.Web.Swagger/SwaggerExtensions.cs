using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;

namespace DNVGL.OAuth.Web.Swagger
{
	public static class SwaggerExtensions
	{
		public static IServiceCollection AddSwagger(this IServiceCollection services, Action<SwaggerOptions> setupAction = null)
		{
			var options = new SwaggerOptions { Enabled = true };

			setupAction?.Invoke(options);

			if (!options.Enabled) return services;

			services.AddSwaggerGen(o =>
			{
				o.SwaggerDoc(options.Version, options.ApiInfo);

				// setup xml comments
				var xmlFilename = $"{Assembly.GetEntryAssembly().GetName().Name}.xml";
				o.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

				// add OAuth2 security scheme
				var oauth2Scheme = new OpenApiSecurityScheme
				{
					Type = SecuritySchemeType.OAuth2,
					Scheme = "OAuth2",
					Reference = new OpenApiReference { Id = "OAuth2", Type = ReferenceType.SecurityScheme },
					Flows = new OpenApiOAuthFlows
					{
						Implicit = options.ImplicitFlow,
						AuthorizationCode = options.AuthCodeFlow,
						ClientCredentials = options.ClientCredsFlow
					},
				};

				if(oauth2Scheme.Flows.Implicit != null)
				{
					ReverseScopes(oauth2Scheme.Flows.Implicit);
				}

				if (oauth2Scheme.Flows.AuthorizationCode != null)
				{
					ReverseScopes(oauth2Scheme.Flows.AuthorizationCode);
				}

				if (oauth2Scheme.Flows.ClientCredentials != null)
				{
					ReverseScopes(oauth2Scheme.Flows.ClientCredentials);
					oauth2Scheme.Flows.ClientCredentials.TokenUrl = new Uri("/swagger/token", UriKind.Relative);
				}

				o.AddSecurityDefinition(oauth2Scheme.Reference.Id, oauth2Scheme);
				o.AddSecurityRequirement(new OpenApiSecurityRequirement { { oauth2Scheme, new string[0] } });

				// add HTTP security scheme
				var httpScheme = new OpenApiSecurityScheme
				{
					Type = SecuritySchemeType.Http,
					Scheme = "Bearer",
					In = ParameterLocation.Header,
					Reference = new OpenApiReference { Id = "Bearer", Type = ReferenceType.SecurityScheme }
				};
				o.AddSecurityDefinition(httpScheme.Reference.Id, httpScheme);
				o.AddSecurityRequirement(new OpenApiSecurityRequirement { { httpScheme, new string[0] } });

				// customize generated content in swagger.json
				o.CustomOperationIds(d =>
				{
					if (d.ActionDescriptor is ControllerActionDescriptor desc)
					{
						return $"{desc.ControllerName}_{desc.ActionName}";
					}

					return null;
				});

				o.OrderActionsBy(d => d.RelativePath);
			});

			return services;

			void ReverseScopes(OpenApiOAuthFlow flow)
			{
				flow.Scopes = flow.Scopes.ToDictionary(p => p.Value, p => p.Key);
			}
		}

		public static IApplicationBuilder UseSwaggerWithUI(this IApplicationBuilder app, Action<SwaggerOptions> setupAction = null)
		{
			var options = new SwaggerOptions { Enabled = true };

			setupAction?.Invoke(options);

			if (!options.Enabled) return app;

			app.UseSwagger()
				.UseSwaggerUI(o =>
				{
					o.SwaggerEndpoint($"{options.Version}/swagger.json", $"{options.ApiInfo?.Title} {options.Version}");

					o.DisplayRequestDuration();

					o.OAuthClientId(options.ClientId);
					o.OAuthClientSecret(options.ClientSecret);

					o.DisplayOperationId();
					o.EnableFilter();

					o.ConfigObject.AdditionalItems.Add("syntaxHighlight", false);
				});

			// client credentials token request proxy for swagger UI
			app.MapWhen(
				context => context.Request.Path.StartsWithSegments("/swagger/token") && context.Request.Method == HttpMethods.Post,
				b => b.Run(async context =>
				{
					var basicKey = context.Request.Headers["authorization"].ToString().Substring(6);
					var clienIdSecret = Encoding.UTF8.GetString(Convert.FromBase64String(basicKey)).Split(':');
					var form = context.Request.Form.ToDictionary(i => i.Key, i => i.Value.ToString());
					form["client_id"] = clienIdSecret[0];
					form["client_secret"] = clienIdSecret[1];
					var tokenUrl = options.ClientCredsFlow.TokenUrl;

					var httpClientFactory = context.RequestServices.GetService<IHttpClientFactory>();
					var httpClient = httpClientFactory?.CreateClient("SwaggerClientProxy") ?? new HttpClient();
					var content = new FormUrlEncodedContent(form);
					var responseMessage = await httpClient.PostAsync(tokenUrl, content);

					context.Response.StatusCode = (int)responseMessage.StatusCode;
					var body = await responseMessage.Content.ReadAsStringAsync();
					context.Response.Headers.Add("Content-Length", body.Length.ToString());
					context.Response.Headers.Add("Content-Type", "application/json; charset=utf-8");
					await context.Response.WriteAsync(body);
				})
			);

			return app;
		}
	}
}
