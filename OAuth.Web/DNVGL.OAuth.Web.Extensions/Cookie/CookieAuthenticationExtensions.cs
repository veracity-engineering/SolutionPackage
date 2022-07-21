using System;
using DNVGL.OAuth.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace DNV.OAuth.Web.Extensions.Cookie
{
	/// <summary>
	/// 
	/// </summary>
	public static class CookieAuthenticationExtensions
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="options"></param>
		/// <param name="apiPredicate"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static CookieAuthenticationOptions SuppressOAuthRedirectForApi(this CookieAuthenticationOptions options,
			Func<HttpRequest, bool>? apiPredicate = null)
		{
			if (options == null)
				throw new ArgumentNullException(nameof(options));

			options.Events ??= new CookieAuthenticationEvents();

			var previous1 = options.Events.OnRedirectToAccessDenied;
			options.Events.OnRedirectToAccessDenied = async ctx =>
			{
				if (apiPredicate?.Invoke(ctx.Request) ?? true &&
				    ctx.Response.StatusCode == StatusCodes.Status200OK)
				{
					ctx.Response.Clear();
					ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
					return;
				}

				if (previous1 != null)
					await previous1(ctx);
			};

			var previous2 = options.Events.OnRedirectToLogin;
			options.Events.OnRedirectToLogin = async ctx =>
			{
				if (apiPredicate?.Invoke(ctx.Request) ?? true &&
				    ctx.Response.StatusCode == StatusCodes.Status200OK)
				{
					ctx.Response.Clear();
					ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
					return;
				}

				if (previous2 != null)
					await previous2(ctx);
			};

			return options;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="services"></param>
		/// <param name="apiPredicate"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static IServiceCollection SuppressOAuthRedirectForApi(this IServiceCollection services,
			Func<HttpRequest, bool>? apiPredicate = null)
		{
			if (services == null)
				throw new ArgumentNullException(nameof(services));
			
			return services.Configure<CookieAuthenticationOptions>(
				CookieAuthenticationDefaults.AuthenticationScheme, 
				o => o.SuppressOAuthRedirectForApi(apiPredicate));
		}
	}		
}
