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
            [PermissionValue(Id = "1", Key = "ManageUser", Name = "ManageUser", Group = "Admin", Description = "ManageUser")]
            ManageUser,

            /// <summary>
            /// Permission of user readonly operation
            /// </summary>
            [PermissionValue(Id = "2", Key = "ViewUser", Name = "ViewUser", Group = "Admin", Description = "ViewUser")]
            ViewUser,

            /// <summary>
            /// Permission of role readonly operation
            /// </summary>
            [PermissionValue(Id = "3", Key = "ViewRole", Name = "ViewRole", Group = "Admin", Description = "ViewRole")]
            ViewRole,

            /// <summary>
            /// Permission of role crud operations
            /// </summary>
            [PermissionValue(Id = "4", Key = "ManageRole", Name = "ManageRole", Group = "Admin", Description = "ManageRole")]
            ManageRole
        }
    }
}
