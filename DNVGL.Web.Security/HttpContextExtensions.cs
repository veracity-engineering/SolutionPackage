using Microsoft.AspNetCore.Http;
using System;
using System.Text;

namespace DNVGL.Web.Security
{
	public static class HttpContextExtensions
	{
		public static string CreateNonce(this HttpContext httpContext)
		{
			var b64RequestId = Convert.ToBase64String(Encoding.UTF8.GetBytes(httpContext.TraceIdentifier));
			return $"'nonce-{b64RequestId}'";
		}
	}
}
