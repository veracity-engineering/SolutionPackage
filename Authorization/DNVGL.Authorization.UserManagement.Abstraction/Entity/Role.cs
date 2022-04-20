// Copyright (c) DNV. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;

namespace DNVGL.Authorization.UserManagement.Abstraction.Entity
{
    /// <summary>
    /// Represents role information for a role record.
    /// </summary>
    public class Role
    {
        /// <summary>
        /// Gets or sets the primary key for this role.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the role name for this role.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description for this role.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating if this role is active or not.
        /// </summary>
        /// <value>True if this role is active, otherwise false.</value>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating if this role is deleted or not.
        /// </summary>
        /// <remarks>
        /// This property is not being used by in the package now. it will be used to support soft delete in future release.
        /// </remarks>
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets the role permissions for this role.
        /// </summary>
        /// <value>Permissions are combined as a string which use semicolon(;) as a delimiter.</value>
        public virtual string Permissions { get; set; }

        /// <summary>
        /// Gets or sets id of the company to which this role belongs.
        /// </summary>
        /// <remarks>
        /// It is null if it is a global role.
        /// </remarks>
        public virtual string CompanyId { get; set; }

        /// <summary>
        /// Gets the company to which this role belongs.
        /// </summary>
        /// <remarks>
        /// <para>Do not set this property to change a role's company. Instead, set <see cref="CompanyId"/>.</para>
        /// <para>It is null if it is a global role.</para>
        /// </remarks>
        public Company Company { get; set; }

        /// <summary>
        /// Gets the company permission list for this role.
        /// </summary>
        public virtual IReadOnlyList<string> PermissionKeys => Permissions.SplitToList(';');

        /// <summary>
        /// Gets or sets the name of user who created this role.
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the UTC date that this role is created.
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the name of user who updated this role last time.
        /// </summary>
        public string UpdatedBy { get; set; }

        /// <summary>
        /// Gets or sets the UTC date that this role is updated last time.
        /// </summary>
        public DateTime UpdatedOnUtc { get; set; }
    }
}
