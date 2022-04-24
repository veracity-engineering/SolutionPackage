// Copyright (c) DNV. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
using System;
using DNVGL.Authorization.UserManagement.Abstraction;
using DNVGL.Authorization.Web.Abstraction;

namespace DNVGL.Authorization.UserManagement.ApiControllers
{
    /// <summary>
    /// Provides an option to configure the user management module.
    /// </summary>
    public class UserManagementOptions
    {
        private UserManagementMode _mode = UserManagementMode.Company_CompanyRole_User;

        /// <summary>
        /// Gets or sets the <see cref="UserManagementMode"/>.
        /// </summary>
        /// <remarks>
        /// By default, it is <see cref="UserManagementMode.Company_CompanyRole_User"/>.
        /// </remarks>
        public UserManagementMode Mode
        {
            get { return _mode; }
            set
            {
                _mode = value;
            }
        }



        /// <summary>
        /// Gets or sets the <see cref="PermissionOptions"/>.
        /// <example>For example, Get user identity from name claim in windows authentication:
        /// <code>
        ///    PermissionOptions = new PermissionOptions{
        ///        GetUserIdentity = (userPrincipal) => userPrincipal.Claims.FirstOrDefault(t => t.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value,
        ///        HandleUnauthorizedAccess = BuiltinUnauthorizedAccessHandler.Return403ForbiddenCode}
        /// </code>
        /// </example>
        /// </summary>
        public PermissionOptions PermissionOptions { get; set; }
    }
}
