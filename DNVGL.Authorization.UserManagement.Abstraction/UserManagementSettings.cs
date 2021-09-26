// Copyright (c) DNV. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace DNVGL.Authorization.UserManagement.Abstraction
{
    /// <summary>
    /// Provides a class to configure the user management business flow.
    /// </summary>
    public class UserManagementSettings
    {
        /// <summary>
        /// Gets or sets the user management business flow.
        /// </summary>
        public UserManagementMode Mode { get; set; }
    }

    /// <summary>
    /// Provide an enum to define supported user management business flow.
    /// </summary>
    public enum UserManagementMode
    {
        /// <summary>
        /// <list type="bullet">
        /// <item>A Role must belongs to a compnay.</item>
        /// <item>A User must belongs to one or mutiple companies.</item>
        /// <item>A User has one or mutiple company roles.</item>
        /// </list>
        /// </summary>
        Company_CompanyRole_User,

        /// <summary>
        /// <list type="bullet">
        /// <item>Roles desn't belong to any compnay. They are global.</item>
        /// <item>A User must belongs to one or mutiple companies.</item>
        /// <item>A User has one or mutiple roles.</item>
        /// </list>
        /// </summary>
        Company_GlobalRole_User,

        /// <summary>
        /// <list type="bullet">
        /// <item>Roles are global.</item>
        /// <item>There is no company.</item>
        /// <item>A User has one or mutiple roles.</item>
        /// </list>
        /// </summary>
        Role_User,
    }
}
