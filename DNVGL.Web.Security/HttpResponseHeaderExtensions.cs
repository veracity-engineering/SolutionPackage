using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace DNVGL.Web.Security
{
    /// <summary>
    /// Extension methods to Response.Headers  
    /// </summary>
    public static class HttpResponseHeaderExtensions
    {
        /// <summary>
        ///<para> Adds and configures the predefined headers for Http response headers.</para> 
        ///<para>TO avoid overwrite your own customized response header settings, call this method  at last. If the predefined headers is not desired, setup you desired headers before calling this method</para> 
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
        /// </list>
        /// </remarks>
        /// <param name="headerDictionary">The Response.Headers</param>
        public static void SetupDefaultHeaders(this IHeaderDictionary headerDictionary)
        {
            headerDictionary.AddKeyOnlyIfNotExists("X-Xss-Protection", "1");
            headerDictionary.AddKeyOnlyIfNotExists("X-Frame-Options", "SAMEORIGIN");
            headerDictionary.AddKeyOnlyIfNotExists("Referrer-Policy", "no-referrer");
            headerDictionary.AddKeyOnlyIfNotExists("X-Content-Type-Options", "nosniff");
            headerDictionary.AddKeyOnlyIfNotExists("X-Permitted-Cross-Domain-Policies", "none");
            headerDictionary.AddKeyOnlyIfNotExists("Expect-CT", "enforce, max-age=7776000");
        }

        private static void AddKeyOnlyIfNotExists(this IHeaderDictionary headerDictionary, string key, string value)
        {
            if (!headerDictionary.ContainsKey(key))
            {
                headerDictionary.Add(key, value);
            }
        }

        internal static void AddContentSecurityPolicy(this IHeaderDictionary headerDictionary)
        {
            if (headerDictionary.ContainsKey("Content-Security-Policy") || headerDictionary.ContainsKey("Content-Security-Policy-Report-Only"))
                return;

            var csp = string.Join("; ", "default-src 'self'", "object-src 'self'", 
                string.Join(" ", "connect-src 'self'", "https://dc.services.visualstudio.com"), 
                string.Join(" ", "script-src 'self'", "https://www.recaptcha.net", "https://www.gstatic.com", "https://www.gstatic.cn"),
                string.Join(" ", "font-src 'self' data:", "https://onedesign.azureedge.net"),
                string.Join(" ", "media-src 'self'"),
                string.Join(" ", "worker-src 'self' blob:"),
                string.Join(" ", "img-src 'self' data:", "https://onedesign.azureedge.net"),
                string.Join(" ", "frame-src 'self'", "https://www.google.com", "https://www.recaptcha.net/"),
                string.Join(" ", "style-src 'self'","https://onedesign.azureedge.net")
                );

            headerDictionary.Add("Content-Security-Policy", new[] { csp });
        }
    }
}
