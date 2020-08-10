using DNVGL.OAuth.Web;
using DNVGL.OAuth.Web.Swagger;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
#if NETCORE3
using Microsoft.Extensions.Hosting;
#endif
using System;
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
			// add authentication for web app
			services.AddOidc(o =>
			{
				this.Configuration.GetSection("Oidc").Bind(o);

				o.Events = new OpenIdConnectEvents
				{
					OnAuthorizationCodeReceived = async context =>
					{
						var code = context.ProtocolMessage.Code;
						var clientApp = MsalAppBuilder.BuildConfidentialClientApplication(o, context.HttpContext, context.TokenEndpointRequest.GetParameter("code_verifier"));

						try
						{
							var authResult = await clientApp.AcquireTokenByAuthorizationCode(o.Scopes, code).ExecuteAsync();
							var account = authResult.Account;
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
					OnTokenValidated = context =>
					{
						var objectId = context.Principal.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
						var tokenKeyClaim = new Claim("TokenKey", $"{objectId}-{o.SignInPolicy}.{o.TenantId}");
						(context.Principal.Identity as ClaimsIdentity)?.AddClaim(tokenKeyClaim);
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
