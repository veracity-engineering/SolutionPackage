using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace DNVGL.OAuth.Web.Extensions.Cookie
{
	public static class CookieAuthenticationExtensions
	{
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="setCookieOption"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static CookieAuthenticationOptions AddCookiePathIsolation(this CookieAuthenticationOptions options, Func<CookieOptions, bool> setCookieOption = null)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

			options.Events = options.Events?? new CookieAuthenticationEvents();

            var previous = options.Events.OnSigningIn;
            options.Events.OnSigningIn = async ctx =>
            {
                if (previous != null)
                    await previous(ctx).ConfigureAwait(false);

                if (!string.IsNullOrEmpty(ctx.Properties.RedirectUri))
					ctx.CookieOptions.Path = ctx.Properties.RedirectUri;

                if (setCookieOption != null && !setCookieOption(ctx.CookieOptions))
	                ctx.CookieOptions.Expires = DateTimeOffset.MinValue;
            };

            return options;
        }
	}
}
