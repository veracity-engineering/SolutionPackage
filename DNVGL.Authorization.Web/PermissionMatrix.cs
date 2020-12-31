using System;
using System.Collections.Generic;
using System.Text;

namespace DNVGL.Authorization.Web
{
    /// <summary>
    /// predefined permission related to user and role management.
    /// </summary>
    public class PermissionMatrix : IPermissionMatrix
    {

        public enum Premissions
        {
            /// <summary>
            /// Permission of user crud operations
            /// </summary>
            [PermissionValue(id: "1", key: "ManageUser", name: "ManageUser", group: "Admin", description: "ManageUser")]
            ManageUser,

            /// <summary>
            /// Permission of user readonly operation
            /// </summary>
            [PermissionValue(id: "2", key: "ViewUser", name: "ViewUser", group: "Admin", description: "ViewUser")]
            ViewUser,

            /// <summary>
            /// Permission of role readonly operation
            /// </summary>
            [PermissionValue(id: "3", key: "ViewRole", name: "ViewRole", group: "Admin", description: "ViewRole")]
            ViewRole,

            /// <summary>
            /// Permission of role crud operations
            /// </summary>
            [PermissionValue(id: "4", key: "ManageRole", name: "ManageRole", group: "Admin", description: "ManageRole")]
            ManageRole
        }
    }
}
