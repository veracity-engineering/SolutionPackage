// Copyright (c) DNV. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DNVGL.Authorization.Web
{
    /// <summary>
    /// Provides a api controller filter to ignore permissions check for user when access their own company's resource.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class CompanyMemberIgnorePermissionFilterAttribute : TypeFilterAttribute
    {
        private readonly string[] _permissionsToIgore;

        /// <summary>
        /// Get a collection of permission to be ignored
        /// </summary>
        public string[] PermissionsToIgore
        {
            get { return _permissionsToIgore; }
        }

        /// <summary>
        /// Constructs a new instance of <see cref="CompanyMemberIgnorePermissionFilterAttribute"/>.
        /// </summary>
        /// <param name="permissionsToIgore">A collection of permission to be ignored.</param>
        public CompanyMemberIgnorePermissionFilterAttribute(params string[] permissionsToIgore)
            : base(typeof(CompanyMemberIgnorePermissionFilterImpl))
        {
            _permissionsToIgore = permissionsToIgore;
            Arguments = new object[] { _permissionsToIgore };
        }

        /// <summary>
        /// Constructs a new instance of <see cref="CompanyMemberIgnorePermissionFilterAttribute"/>.
        /// </summary>
        /// <param name="permissionsToIgore">A collection of permission to be ignored.</param>
        public CompanyMemberIgnorePermissionFilterAttribute(params object[] permissionsToIgore)
            : base(typeof(CompanyMemberIgnorePermissionFilterImpl))
        {
            _permissionsToIgore = permissionsToIgore.Select(x => (x as Enum).GetPermissionKey()).ToArray();
            Arguments = new object[] { _permissionsToIgore };
        }

        private class CompanyMemberIgnorePermissionFilterImpl : IAsyncActionFilter
        {
            private readonly string[] _permissionsToIgore;

            public CompanyMemberIgnorePermissionFilterImpl(string[] permissionsToIgore)
            {
                _permissionsToIgore = permissionsToIgore;
            }


            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                if (_permissionsToIgore != null && _permissionsToIgore.Length>0)
                {
                    context.HttpContext.Request.Headers.Remove("AUTHORIZATION.COMPANY.IGNORE.PERMISSIONS");
                    context.HttpContext.Request.Headers.Add("AUTHORIZATION.COMPANY.IGNORE.PERMISSIONS", string.Join(',', _permissionsToIgore));
                }

                await next();
            }
        }
    }
}
