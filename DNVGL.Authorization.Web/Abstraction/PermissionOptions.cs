// Copyright (c) DNV. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace DNVGL.Authorization.Web.Abstraction
{
    /// <summary>
    /// Provides a class to configure permission check behaviors.
    /// </summary>
    public class PermissionOptions
    {
        /// <summary>
        /// A Function to get user's identity id from <see cref="ClaimsPrincipal"/>.
        /// </summary>
        public Func<ClaimsPrincipal, string> GetUserIdentity { get; set; }

        /// <summary>
        /// A Function to get compnay's primary key from <see cref="HttpContext"/>.
        /// </summary>
        public Func<HttpContext, string> GetCompanyIdentity { get; set; }

        /// <summary>
        /// An action to handle unauthorized access.
        /// </summary>
        public Action<HttpContext, string> HandleUnauthorizedAccess { get; set; }
    }

    /// <summary>
    /// Provides a built in implementation of unauthorized access behaviors.
    /// </summary>
    public static class BuiltinUnauthorizedAccessHandler
    {
        /// <summary>
        /// Return 403 code to the client.
        /// </summary>
        public static readonly Action<HttpContext, string> Return403ForbiddenCode = (httpContext, missedPermission) =>
        {
            httpContext.Response.StatusCode = 403;
            httpContext.Response.ContentType = "application/text";
            httpContext.Response.Headers.Remove("Cache-Control");
            httpContext.Response.Headers.Add("Cache-Control", "no-cache, no-store");
            httpContext.Response.WriteAsync($"miss permissions: {missedPermission}.");
        };

        /// <summary>
        /// Throw unauthorized access exception directly.
        /// </summary>
        public static readonly Action<HttpContext, string> ThrowExceptionDirectly = (httpContext, missedPermission) =>
        {
            throw new UnauthorizedAccessException($"miss permissions: {missedPermission}.");
        };
    }
}
