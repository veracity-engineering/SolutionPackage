// Copyright (c) DNV. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace DNVGL.Authorization.Web
{
    /// <summary>
    /// Provides a predefined permission related to user management.
    /// </summary>
    public class PermissionMatrix : IPermissionMatrix
    {

        public enum Premissions
        {
            /// <summary>
            /// Permission of user crud operations
            /// </summary>
            [PermissionValue(id: "1000.1", key: "ManageUser", name: "Manage User", group: "Admin", description: "ManageUser")]
            ManageUser,

            /// <summary>
            /// Permission of user readonly operation
            /// </summary>
            [PermissionValue(id: "1000.2", key: "ViewUser", name: "View User", group: "Admin", description: "ViewUser")]
            ViewUser,

            /// <summary>
            /// Permission of role readonly operation
            /// </summary>
            [PermissionValue(id: "1000.3", key: "ViewRole", name: "View Role", group: "Admin", description: "ViewRole")]
            ViewRole,

            /// <summary>
            /// Permission of role crud operations
            /// </summary>
            [PermissionValue(id: "1000.4", key: "ManageRole", name: "Manage Role", group: "Admin", description: "ManageRole")]
            ManageRole,

            /// <summary>
            /// Permission of role readonly operation
            /// </summary>
            [PermissionValue(id: "1000.5", key: "ViewCompany", name: "View Company", group: "Admin", description: "ViewCompany")]
            ViewCompany,

            /// <summary>
            /// Permission of role crud operations
            /// </summary>
            [PermissionValue(id: "1000.6", key: "ManageCompany", name: "Manage Company", group: "Admin", description: "ManageCompany")]
            ManageCompany
        }
    }
}
