using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;

namespace DNVGL.Web.Security
{
    /// <summary>
    /// Default Response Headers extensions to <see cref="IApplicationBuilder"/>.
    /// </summary>
    public static class ResponseHeadersMiddlewareExtensions
    {
        /// <summary>
        ///<para> Adds and configures the predefined headers for Http response headers. Content-Security-Policy header is not added to request which url contains 'swagger'</para> 
        ///<para>TO avoid overwrite your own customized response header settings, call this method  at last. If the predefined headers is not desired, setup you desired headers before calling this method</para> 
        /// <example>
        /// This sample shows how to call the <see cref="UseDefaultHeaders"/> method in the Configure method of Startup class.
        /// <list type="bullet">
        /// <item>
        /// No customized response headers is required:  
        /// <code>
        /// app.UseDefaultHeaders();
        /// </code>
        /// </item>
        /// <item>
        /// To customize X-Frame-Options in response headers:
        /// <code>
        ///  <para/>app.UseDefaultHeaders(h =>
        ///  <para/>{
        ///  <para/>h.Add("X-Frame-Options", "DENNY");;
        ///  <para/>});
        /// </code>
        /// </item>
        /// <item>
        /// To customize csp in response headers:
        /// <code>
        ///  <para/>app.UseDefaultHeaders(h =>
        ///  <para/>{
        ///  <para/>h.AddContentSecurityPolicy(styleSrc: "'self' 'nonce-123456789909876543ghjklkjvcvbnm'");
        ///  <para/>});
        /// </code>
        /// </item>
        /// </list>
        /// </example>
        /// </summary>
        /// <remarks>
        /// <para>The prefined security headers includes:</para>
        /// <list type="bullet">
        /// <item>
        /// <description>X-Xss-Protection = 1</description>
        /// </item>
        /// <item>
        /// <description>X-Frame-Options = SAMEORIGIN</description>
        /// </item>
        /// <item>
        /// <description>Referrer-Policy = no-referrer</description>
        /// </item>
        /// <item>
        /// <description>X-Content-Type-Options = nosniff</description>
        /// </item>
        /// <item>
        /// <description>X-Permitted-Cross-Domain-Policies = none</description>
        /// </item>
        /// <item>
        /// <description>Expect-CT = enforce, max-age=7776000</description>
        /// </item>
        /// <item>
        /// <description>X-Xss-Protection = 1</description>
        /// </item>
        /// <item>
        /// <description>Content-Security-Policy = default-src 'self'; object-src 'self'; connect-src 'self' https://dc.services.visualstudio.com; script-src 'self' https://www.recaptcha.net https://www.gstatic.com https://www.gstatic.cn; font-src 'self' data: https://onedesign.azureedge.net; media-src 'self'; img-src 'self' data: https://onedesign.azureedge.net; frame-src 'self' https://www.google.com https://www.recaptcha.net/;style-src 'self' https://onedesign.azureedge.net;worker-src 'self' blob:</description>
        /// </item>
        /// </list>
        /// </remarks>
        /// <param name="makeHeaders">make your own response headers, It will overwrite the default headers.</param>
        /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
        public static IApplicationBuilder UseDefaultHeaders(this IApplicationBuilder builder, Action<IHeaderDictionary> makeHeaders = null)
        {
            return builder.Use(async (context, next) =>
            {
                makeHeaders?.Invoke(context.Response.Headers);
                context.Response.Headers.SetupDefaultHeaders();
                if (!context.Request.Path.ToString().ToLowerInvariant().Contains("/swagger/"))
                {
                    context.Response.Headers.AddContentSecurityPolicy();
                }
                await next();
            });
        }
    }
}
