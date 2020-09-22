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

        /// <summary>
        /// <para> Add your own Content-Security-Policy by passing value to specified parameters</para> 
        /// <example>
        /// This sample shows how to call the <see cref="AddContentSecurityPolicy"/> method to overwrite specific csp.
        /// <code>
        ///  <para/>app.UseDefaultHeaders(h =>
        ///  <para/>{
        ///  <para/>h.AddContentSecurityPolicy(styleSrc: "'self' 'nonce-123456789909876543ghjklkjvcvbnm'");
        ///  <para/>});
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="headerDictionary">The Response.Headers</param>
        /// <param name="defaultSrc">The value of default-src, default value is 'self'</param>
        /// <param name="objectSrc">The value of object-src, default value is 'self'</param>
        /// <param name="connectSrc">The value of connect-src, default value is 'self' https://dc.services.visualstudio.com</param>
        /// <param name="scriptSrc">The value of script-src, default value is 'self'  https://www.recaptcha.net https://www.gstatic.com https://www.gstatic.cn</param>
        /// <param name="fontSrc">The value of font-src, default value is 'self' data: https://onedesign.azureedge.net</param>
        /// <param name="mediaSrc">The value of media-src, default value is 'self'</param>
        /// <param name="workerSrc">The value of worker-src, default value is 'self' blob:</param>
        /// <param name="imgSrc">The value of img-src, default value is 'self' data: https://onedesign.azureedge.net</param>
        /// <param name="frameSrc">The value of frame-src, default value is 'self' https://www.google.com https://www.recaptcha.net/</param>
        /// <param name="styleSrc">The value of style-src, default value is 'self' https://onedesign.azureedge.net</param>
        public static void AddContentSecurityPolicy(this IHeaderDictionary headerDictionary
            , string defaultSrc = "'self'"
            , string objectSrc = "'self'"
            , string connectSrc = "'self' https://dc.services.visualstudio.com"
            , string scriptSrc = "'self' https://www.recaptcha.net https://www.gstatic.com https://www.gstatic.cn"
            , string fontSrc = "'self' data: https://onedesign.azureedge.net"
            , string mediaSrc = "'self'"
            , string workerSrc = "'self' blob:"
            , string imgSrc = "'self' data: https://onedesign.azureedge.net"
            , string frameSrc = "'self' https://www.google.com https://www.recaptcha.net/"
            , string styleSrc = "'self' https://onedesign.azureedge.net")
        {
            if (headerDictionary.ContainsKey("Content-Security-Policy") || headerDictionary.ContainsKey("Content-Security-Policy-Report-Only"))
                return;

            var csp = string.Join("; "
                , $"default-src {defaultSrc}"
                , $"object-src {objectSrc}"
                , $"connect-src {connectSrc}"
                , $"script-src {scriptSrc}"
                , $"font-src {fontSrc}"
                , $"media-src {mediaSrc}"
                , $"worker-src {workerSrc}"
                , $"img-src {imgSrc}"
                , $"frame-src {frameSrc}"
                , $"style-src {styleSrc}");

            headerDictionary.Add("Content-Security-Policy", new[] { csp });
        }
    }
}
