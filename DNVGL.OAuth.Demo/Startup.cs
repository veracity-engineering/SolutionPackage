using DNVGL.OAuth.Web;
using DNVGL.OAuth.Web.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
#if NETCORE3
using Microsoft.Extensions.Hosting;
#endif
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DNVGL.OAuth.Demo
{
	public class Startup
	{
		public const string ObjectId = "http://schemas.microsoft.com/identity/claims/objectidentifier";

		public IConfiguration Configuration { get; }

		public Startup(IConfiguration configuration) { this.Configuration = configuration; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddInMemoryTokenCaches();

			// add authentication for web app
			services.AddOidc(o =>
			{
				this.Configuration.GetSection("Oidc").Bind(o);

				o.Events = new Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectEvents
				{
					OnAuthorizationCodeReceived = async context =>
					{
						var code = context.ProtocolMessage.Code;
						//var extraQueryParameters = $"code_verifier={context.TokenEndpointRequest.GetParameter("code_verifier")}";

						var clientApp = MsalAppBuilder.BuildConfidentialClientApplication(o, context.HttpContext);

						try
						{
							var authResult = await clientApp.AcquireTokenByAuthorizationCode(o.Scopes, code).ExecuteAsync();
							var account = authResult.Account;
							(context.HttpContext.User.Identity as ClaimsIdentity).AddClaim(new Claim(ObjectId, account.HomeAccountId.Identifier));
							var accounts = await clientApp.GetAccountsAsync();
							account = await clientApp.GetAccountAsync(account.HomeAccountId.Identifier);
							// AccessToken may be relayed as bearer token and made available to APIs
							context.HandleCodeRedemption(authResult.AccessToken, authResult.IdToken);
						}
						catch (Exception ex)
						{
							//TODO: Handle
							throw;
						}
					},
					OnTokenValidated = context => {
						return Task.CompletedTask;
					}
				};
			})
				// add authentication for web api
				.AddJwt(this.Configuration.GetSection("OidcOptions").GetChildren());

#if NETCORE2
			services.AddMvc();
#elif NETCORE3
			services.AddControllersWithViews();
#endif

			// add swagger generation and swagger UI with authentication feature
			services.AddSwagger(o => this.Configuration.GetSection("SwaggerOption").Bind(o));
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
#if NETCORE2
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
#elif NETCORE3

		public void Configure(IApplicationBuilder app, IHostEnvironment env)
#endif
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

#if NETCORE2
			app.UseHttpsRedirection();
			app.UseAuthentication();
			app.UseMvc(routes =>
			{
				routes.MapRoute(name: "default", template: "{controller=Home}/{action=Index}/{id?}");
			});
#elif NETCORE3
			app.UseHttpsRedirection().UseRouting();
			app.UseAuthentication().UseAuthorization();
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
			});
#endif

			// provide parameters to swagger UI
			app.UseSwaggerWithUI(o => this.Configuration.GetSection("SwaggerOption").Bind(o));
		}
	}
}
