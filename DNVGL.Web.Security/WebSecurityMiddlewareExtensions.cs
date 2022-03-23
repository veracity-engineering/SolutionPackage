using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

using System;
using System.Text.RegularExpressions;

namespace DNVGL.Web.Security
{
	/// <summary>
	/// Default Response Headers extensions to <see cref="IApplicationBuilder"/>.
	/// </summary>
	public static class WebSecurityMiddlewareExtensions
	{
		/// <summary>
		/// <para> Adds and configures the predefined headers for Http response headers. Content-Security-Policy header is not added to request which url contains 'swagger'</para> 
		/// <para>To avoid overwrite your own customized response header settings, call this method at last. If the predefined headers is not desired, setup you desired headers before calling this method</para> 
		/// <para>To remove 'server' header on Kestrel Server, add the folowing code into ConfigureService method: services.PostConfigure&lt;KestrelServerOptions&gt;(t => t.AddServerHeader = false);</para>
		/// </summary>
		/// <remarks>
		/// <para>The prefined security headers includes:</para>
		/// <list type="bullet">
		/// <item>X-Xss-Protection: 1</item>
		/// <item>X-Frame-Options: SAMEORIGIN</item>
		/// <item>Referrer-Policy: no-referrer</item>
		/// <item>X-Content-Type-Options: nosniff</item>
		/// <item>X-Permitted-Cross-Domain-Policies: none</item>
		/// <item>Expect-CT: enforce, max-age=7776000</item>
		/// <item>X-Xss-Protection: 1</item>
		/// <item>Content-Security-Policy: 
		/// <list type="bullet">
		///		<item>connect-src 'self' https://dc.services.visualstudio.com https://login.microsoftonline.com https://login.veracity.com https://loginstag.veracity.com https://logintest.veracity.com; </item>
		///		<item>default-src 'self'; </item>
		///		<item>font-src 'self' data: https://onedesign.azureedge.net; </item>
		///		<item>frame-src 'self' https://www.google.com https://www.recaptcha.net/; </item>
		///		<item>img-src 'self' data: https://onedesign.azureedge.net; </item>
		///		<item>media-src 'self'; </item>
		///		<item>object-src 'self' {nonce}; </item>
		///		<item>script-src 'self' https://www.recaptcha.net https://www.gstatic.com https://www.gstatic.cn {nonce}; </item>
		///		<item>style-src 'self' https://onedesign.azureedge.net {nonce}; </item>
		///		<item>worker-src 'self' blob:</item>
		///	</list>
		///	</item>
		/// </list>
		/// </remarks>
		/// <param name="builder"></param>
		/// <param name="apiPredicate"></param>
		/// <param name="exceptionPredicate"></param>
		/// <param name="customizeHeaders"></param>
		/// <returns>The <see cref="IApplicationBuilder"/>.</returns>
		public static IApplicationBuilder UseDefaultSecurityHeaders(this IApplicationBuilder builder, Func<HttpRequest, bool> apiPredicate = null, Func<HttpRequest, bool> exceptionPredicate = null, Action<IHeaderDictionary> customizeHeaders = null)
		{
			return builder.Use(async (context, next) =>
			{
				context.Response.SetDefaultSecurityHeaders(apiPredicate, exceptionPredicate);
				customizeHeaders?.Invoke(context.Response.Headers);
				await next();
			});
		}

		public static IApplicationBuilder UseSecutiryHeaders(this IApplicationBuilder builder, Func<HttpContext, string> setupCSP, Func<HttpRequest, bool> apiPredicate = null, Func<HttpRequest, bool> exceptionPredicate = null)
		{
			return builder.Use(async (context, next) =>
			{
				context.Response.SetSecurityHeaders(setupCSP, apiPredicate, exceptionPredicate);
				await next();
			});
		}
	}
}