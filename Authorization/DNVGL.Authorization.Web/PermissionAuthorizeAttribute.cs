// Copyright (c) DNV. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace DNVGL.Authorization.Web
{
    /// <summary>
    /// Provides a api controller action's decoartor to specify access permission.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class PermissionAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly string[] _permissionsToCheck;

        /// <summary>
        /// Get the collection of required permissions
        /// </summary>
        public string[] PermissionsToCheck
        {
            get { return _permissionsToCheck; }
        }

        /// <summary>
        /// Constructs a new instance of <see cref="PermissionAuthorizeAttribute"/>
        /// </summary>
        /// <param name="permissionsToCheck">A collection of required permissions.</param>
        public PermissionAuthorizeAttribute(params string[] permissionsToCheck) : base("PermissionAuthorize")
        {
            _permissionsToCheck = permissionsToCheck;
        }

        /// <summary>
        /// Constructs a new instance of <see cref="PermissionAuthorizeAttribute"/>
        /// </summary>
        /// <param name="permissionsToCheck">A collection of required permissions.</param>
        public PermissionAuthorizeAttribute(params object[] permissionsToCheck) : base("PermissionAuthorize") 
        {
            _permissionsToCheck = permissionsToCheck.Select(x => (x as Enum).GetPermissionKey()).ToArray();
        }
    }
}
