using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace DNV.OAuth.Web.Extensions.Multitenancy
{
	/// <summary>
	/// 
	/// </summary>
	public static class MtExtensions
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="appBuilder"></param>
		/// <param name="whenToIgnore"></param>
		/// <returns></returns>
		public static IApplicationBuilder UseMultitenancy(this IApplicationBuilder appBuilder, Func<HttpRequest, bool>? whenToIgnore = null)
		{
			return appBuilder.UseMiddleware<TenantResolutionMiddleware>(whenToIgnore ?? (_ => false));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="services"></param>
		/// <param name="configAction"></param>
		public static void AddMultitenantAuthentication(
			this IServiceCollection services,
			Action<CookieAuthenticationOptions, HttpContext>? configAction = null)
		{
			services.AddSingleton<IPostConfigureOptions<OpenIdConnectOptions>, MtOidcPostConfigureOptions>();
			services.AddSingleton<IOptionsMonitor<CookieAuthenticationOptions>, MtCookieOptionsMonitor>();
			services.AddSingleton<IConfigureOptions<CookieAuthenticationOptions>>(sp =>
			{
				var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
				return new MtCookieConfigureNamedOptions(
					httpContextAccessor, 
					configAction?? ((options, context) =>
					{
						var tenant = context.Request.PathBase.Value?.Trim('/');

						options.DataProtectionProvider = context
							.RequestServices.GetRequiredService<IDataProtectionProvider>()
							.CreateProtector($"{tenant}.Data.Protection");

						options.Cookie.Name = $"{tenant}.Oidc.Cookie";
					}));
			});
		}
	}
}
