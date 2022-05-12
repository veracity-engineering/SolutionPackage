using System;
using DNVGL.OAuth.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;

namespace DNV.OAuth.Web.Extensions.Cookie
{
	public static class CookieAuthenticationExtensions
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="options"></param>
		/// <param name="shouldPersistCookie"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static CookieAuthenticationOptions AddCookiePathIsolation(this CookieAuthenticationOptions options, Func<CookieOptions, bool>? shouldPersistCookie = null)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

			options.Events = options.Events?? new CookieAuthenticationEvents();
			
			var handler = options.Events.OnSigningIn;
            options.Events.OnSigningIn = async ctx =>
            {
                var originalPath = ctx.Properties.GetRequestPath();
                if (!string.IsNullOrEmpty(originalPath))
	                ctx.CookieOptions.Path = originalPath;
				
                if (shouldPersistCookie != null && !shouldPersistCookie(ctx.CookieOptions))
	                ctx.CookieOptions.Expires = DateTimeOffset.MinValue;

                if (handler != null)
	                await handler(ctx).ConfigureAwait(false);
			};

            return options;
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="options"></param>
		/// <param name="isApiRequest"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static CookieAuthenticationOptions SuppressRedirectForApi(this CookieAuthenticationOptions options, Func<HttpRequest, bool> isApiRequest)
        {
	        if (options == null)
		        throw new ArgumentNullException(nameof(options));
	        if (isApiRequest == null)
		        throw new ArgumentNullException(nameof(isApiRequest));
	        if (options.Events == null)
				options.Events = new CookieAuthenticationEvents();

	        var handlerDenied = options.Events.OnRedirectToAccessDenied;
	        options.Events.OnRedirectToAccessDenied = async ctx =>
	        {
		        if (isApiRequest(ctx.Request) && ctx.Response.StatusCode == StatusCodes.Status200OK) 
		        {
			        ctx.Response.Clear();
			        ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
			        return;
		        }

		        if (handlerDenied != null)
			        await handlerDenied(ctx).ConfigureAwait(false);
	        };

	        var handlerLogin = options.Events.OnRedirectToLogin;
	        options.Events.OnRedirectToLogin = async ctx =>
	        {
		        if (isApiRequest(ctx.Request) && ctx.Response.StatusCode == StatusCodes.Status200OK)
		        {
			        ctx.Response.Clear();
			        ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
			        return;
		        }

		        if (handlerLogin != null)
			        await handlerLogin(ctx).ConfigureAwait(false);
	        };

	        return options;
        }

		internal const string VeracityRequestPathKey = nameof(VeracityRequestPathKey);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="properties"></param>
		/// <returns></returns>
		internal static string GetRequestPath(this AuthenticationProperties properties)
		{
			if (properties == null)
				throw new ArgumentNullException(nameof(properties));

			return properties.Items.TryGetValue(VeracityRequestPathKey, out var cookiePath)
				? cookiePath : null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="properties"></param>
		/// <param name="cookiePath"></param>
		/// <returns></returns>
		internal static void SetRequestPath(this AuthenticationProperties properties, string cookiePath)
		{
			if (properties == null)
				throw new ArgumentNullException(nameof(properties));

			if (string.IsNullOrEmpty(cookiePath))
			{
				properties.Items.Remove(VeracityRequestPathKey);
				return;
			}

			properties.Items[VeracityRequestPathKey] = cookiePath;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="oidc"></param>
		/// <exception cref="ArgumentNullException"></exception>
		public static OidcOptions CarryRequestPathOverIdp(this OidcOptions oidc)
		{
			if (oidc == null)
				throw new ArgumentNullException(nameof(oidc));

			if (oidc.Events == null)
				oidc.Events = new OpenIdConnectEvents();

			var handler = oidc.Events.OnRedirectToIdentityProvider;
			oidc.Events.OnRedirectToIdentityProvider = async ctx =>
			{
				var path = ctx.Properties.RedirectUri.Trim('/');
				if (!string.IsNullOrEmpty(path))
				{
					int slash, question;
					if ((slash = path.IndexOf('/')) != -1)
						path = path.Substring(0, slash);
					else if ((question = path.IndexOf('?')) != -1)
						path = path.Substring(0, question);
					ctx.Properties.SetRequestPath($"/{path}");
				}

				if (handler != null)
					await handler(ctx).ConfigureAwait(false);
			};

			return oidc;
		}
	}
}
