using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace DNVGL.Authorization.Web.Abstraction
{
    public class PermissionOptions
    {
        public Func<HttpContext, string> GetUserIdentity;
        public Action<HttpContext, string> HandleUnauthorizedAccess;
    }

    public static class BuiltinUnauthorizedAccessHandler
    {
        public static readonly Action<HttpContext, string> Return403ForbiddenCode = (httpContext, missedPermission) =>
        {
            httpContext.Response.StatusCode = 403;
            httpContext.Response.ContentType = "application/text";
            httpContext.Response.Headers.Remove("Cache-Control");
            httpContext.Response.Headers.Add("Cache-Control", "no-cache, no-store");
            httpContext.Response.WriteAsync($"miss permissions: {missedPermission}.");
        };

        public static readonly Action<HttpContext, string> ThrowExceptionDirectly = (httpContext, missedPermission) =>
        {
            throw new UnauthorizedAccessException($"miss permissions: {missedPermission}.");
        };
    }
}
