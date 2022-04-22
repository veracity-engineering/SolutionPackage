using DNVGL.Web.Security.CSP;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace DNVGL.Web.Security
{
	public static class HttpContextExtensions
	{
		public static string CreateNonce(this HttpContext source)
		{
			var b64RequestId = Convert.ToBase64String(Encoding.UTF8.GetBytes(source.TraceIdentifier));
			return $"'nonce-{b64RequestId}'";
		}

		public static void Set(this IHeaderDictionary source, string name, string value)
		{
			if (source.ContainsKey(name)) source.Remove(name);

			source.Add(name, value);
		}

		public static void Set(this IHeaderDictionary source, Dictionary<string, string> newHeaders)
		{
			if (newHeaders == null) throw new ArgumentNullException(nameof(newHeaders));

			foreach (var key in newHeaders.Keys) source.Set(key, newHeaders[key]);
		}

		public static void SetDefault(this IHeaderDictionary source, string csp)
		{
			var securityHeaders = new Dictionary<string, string>
			{
				{ "Expect-CT", "enforce, max-age=7776000" },
				{ "Referrer-Policy", "no-referrer" },
				{ "X-Permitted-Cross-Domain-Policies", "none" },
				{ "X-Xss-Protection", "1" },
				{ ContentSecurityPolicy.Name, csp },
				{ "Strict-Transport-Security", "max-age=15552000; includeSubDomains" },
				{ "X-Content-Type-Options", "nosniff" },
				{ "X-Frame-Options", "SAMEORIGIN" },
			};

			source.Set(securityHeaders);
		}

		public static void SetDefaultForApi(this IHeaderDictionary source)
		{
			var securityHeaders = new Dictionary<string, string>
			{
				{ "Cache-Control", "no-store" },
				{ ContentSecurityPolicy.Name, "frame-ancestors 'none'" },
				{ "Strict-Transport-Security", "max-age=15552000; includeSubDomains" },
				{ "X-Content-Type-Options", "nosniff" },
				{ "X-Frame-Options", "DENY" },
			};

			source.Set(securityHeaders);
		}

		public static HttpResponse SetDefaultSecurityHeaders(this HttpResponse source, Func<HttpRequest, bool> apiPredicate = null, Func<HttpRequest, bool> exceptionPredicate = null)
		{
			var request = source.HttpContext?.Request;

			if (exceptionPredicate?.Invoke(request) == true)
			{
				return source;
			}

			if (apiPredicate?.Invoke(request) == true)
			{
				source.Headers.SetDefaultForApi();
			}
			else
			{
				var nonce = source.HttpContext?.CreateNonce();
				var csp = ContentSecurityPolicy.CreateDefault(nonce);
				source.Headers.SetDefault(csp.GetValue());
			}

			return source;
		}

		public static HttpResponse SetSecurityHeaders(this HttpResponse source, Func<HttpContext, string> setupCSP, Func<HttpRequest, bool> apiPredicate = null, Func<HttpRequest, bool> exceptionPredicate = null)
		{
			if(setupCSP == null) throw new ArgumentNullException(nameof(setupCSP));

			var request = source.HttpContext?.Request;

			if (exceptionPredicate?.Invoke(request) == true)
			{
				return source;
			}

			if (apiPredicate?.Invoke(request) == true)
			{
				source.Headers.SetDefaultForApi();
			}
			else
			{
				var csp = setupCSP(source.HttpContext) ?? ContentSecurityPolicy.CreateDefault().GetValue();
				source.Headers.SetDefault(csp);
			}

			return source;
		}
	}
}
