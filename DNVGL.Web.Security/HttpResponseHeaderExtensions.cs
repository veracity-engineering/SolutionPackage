using Microsoft.AspNetCore.Http;
using System;

namespace DNVGL.Web.Security
{
	/// <summary>
	/// Extension methods to Response.Headers  
	/// </summary>
	public static class HttpResponseHeaderExtensions
    {
        private const string DefaultSrc = "'self'";
        private const string ObjectSrc = "'self'";
        private const string ConnectSrc = "'self' https://dc.services.visualstudio.com https://login.veracity.com https://login.microsoftonline.com";
        private const string ScriptSrc = "'self' https://www.recaptcha.net https://www.gstatic.com https://www.gstatic.cn";
        private const string FontSrc = "'self' data: https://onedesign.azureedge.net";
        private const string MediaSrc = "'self'";
        private const string WorkerSrc = "'self' blob:";
        private const string ImgSrc = "'self' https://onedesign.azureedge.net";
        private const string FrameSrc = "'self' https://www.google.com https://www.recaptcha.net/";
        private const string StyleSrc = "'self' https://onedesign.azureedge.net";
        private static string csp = string.Empty;
        private static Func<HttpRequest, bool> SkipRequest = (req) => req.Path.ToString().ToLowerInvariant().Contains("/swagger/");
        /// <summary>
        ///<para> Adds and configures the predefined headers for Http response headers.</para> 
        ///<para>To avoid overwrite your own customized response header settings, call this method  at last. If the predefined headers is not desired, setup you desired headers before calling this method</para> 
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
        /// <description>Strict-Transport-Security = max-age=15552000; includeSubDomains</description>
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
            headerDictionary.AddKeyOnlyIfNotExists("Strict-Transport-Security", "max-age=15552000; includeSubDomains");
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
        ///  <para/>h.ReplaceDefaultContentSecurityPolicy(styleSrc: "'self' 'nonce-123456789909876543ghjklkjvcvbnm'");
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
        public static void ReplaceDefaultContentSecurityPolicy(this IHeaderDictionary headerDictionary
            , string defaultSrc = DefaultSrc
            , string objectSrc = ObjectSrc
            , string connectSrc = ConnectSrc
            , string scriptSrc = ScriptSrc
            , string fontSrc = FontSrc
            , string mediaSrc = MediaSrc
            , string workerSrc = WorkerSrc
            , string imgSrc = ImgSrc
            , string frameSrc = FrameSrc
            , string styleSrc = StyleSrc)
        {
            if (headerDictionary.ContainsKey("Content-Security-Policy") || headerDictionary.ContainsKey("Content-Security-Policy-Report-Only"))
                return;

            if (string.IsNullOrEmpty(csp))
            {
                csp = string.Join("; "
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
            }


            headerDictionary.Add("Content-Security-Policy", new[] { csp });
        }

        private static void PutDefaultContentSecurityPolicy(this IHeaderDictionary headerDictionary)
        {
            headerDictionary.ReplaceDefaultContentSecurityPolicy();
        }

        /// <summary>
        /// <para> Add your own Content-Security-Policy by passing value to specified parameters</para> 
        /// <example>
        /// This sample shows how to call the <see cref="ExtendDefaultContentSecurityPolicy"/> method to overwrite specific csp.
        /// <code>
        ///  <para/>app.UseDefaultHeaders(h =>
        ///  <para/>{
        ///  <para/>h.ExtendDefaultContentSecurityPolicy(styleSrc: "'nonce-123456789909876543ghjklkjvcvbnm'");
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
        public static void ExtendDefaultContentSecurityPolicy(this IHeaderDictionary headerDictionary
            , string defaultSrc = ""
            , string objectSrc = ""
            , string connectSrc = ""
            , string scriptSrc = ""
            , string fontSrc = ""
            , string mediaSrc = ""
            , string workerSrc = ""
            , string imgSrc = ""
            , string frameSrc = ""
            , string styleSrc = "")
        {
            if (headerDictionary.ContainsKey("Content-Security-Policy") || headerDictionary.ContainsKey("Content-Security-Policy-Report-Only"))
                return;

            if (string.IsNullOrEmpty(csp))
            {
                csp = string.Join("; "
                    , $"default-src {DefaultSrc} {defaultSrc}"
                    , $"object-src {ObjectSrc} {objectSrc}"
                    , $"connect-src {ConnectSrc} {connectSrc}"
                    , $"script-src {ScriptSrc} {scriptSrc}"
                    , $"font-src {FontSrc} {fontSrc}"
                    , $"media-src {MediaSrc} {mediaSrc}"
                    , $"worker-src {WorkerSrc} {workerSrc}"
                    , $"img-src {ImgSrc} {imgSrc}"
                    , $"frame-src {FrameSrc} {frameSrc}"
                    , $"style-src {StyleSrc} {styleSrc}");
            }


            headerDictionary.Add("Content-Security-Policy", new[] { csp });
        }


        internal static void AddContentSecurityPolicy(this IHeaderDictionary headerDictionary, HttpRequest httpRequest)
        {
            if (!SkipRequest(httpRequest))
            {
                headerDictionary.PutDefaultContentSecurityPolicy();
            }
            else 
            {
                var cspKey = "Content-Security-Policy";
                if (headerDictionary.ContainsKey(cspKey))
                {
                    headerDictionary.Remove(cspKey);
                }
            }
        }

        /// <summary>
        /// <para>Not add Content-Security-Policy header for specific requests. By default it doesn't add csp for request url which contains word 'swagger'.</para> 
        /// <example>
        /// This sample shows how to call the method to skip csp for specific requests.
        /// <code>
        ///  <para/>app.UseDefaultHeaders(h =>
        ///  <para/>{
        ///  <para/>h.SkipContentSecurityPolicyForRequests((req) => req.Path.ToString().ToLowerInvariant().Contains("/swagger/"));
        ///  <para/>});
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="headerDictionary">The Response.Headers</param>
        /// <param name="skipRequest">Predict function which accept <see cref="HttpRequest"/> as a parameter and return true if CSP should be skipped for the request.</param>
        public static void SkipContentSecurityPolicyOnRequests(this IHeaderDictionary headerDictionary, Func<HttpRequest,bool> skipRequest)
        {
            SkipRequest = skipRequest;
        }


    }
}
