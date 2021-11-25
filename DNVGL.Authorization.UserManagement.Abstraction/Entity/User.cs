// Copyright (c) DNV. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;

namespace DNVGL.Authorization.UserManagement.Abstraction.Entity
{
    /// <summary>
    /// Represents user information for a user record.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets or sets the primary key for this user.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the email for this user.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the first name for this user.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name for this user.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the identity id for this user.
        /// </summary>
        /// <remarks>
        /// It is an id provided by identity provider. 
        /// </remarks>
        public string VeracityId { get; set; }

        /// <summary>
        /// Gets or sets the description for this user.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets id of roles which this user has.
        /// </summary>
        /// <value>Role's ids are combined as a string which use semicolon(;) as a delimiter.</value>
        public virtual string RoleIds { get; set; }

        /// <summary>
        /// Get the roles id list of roles which this user has.
        /// </summary>
        public virtual IReadOnlyList<string> RoleIdList => RoleIds.SplitToList(';');

        /// <summary>
        /// Gets or sets id of company to which this user belongs.
        /// </summary>
        /// <value>Company's ids are combined as a string which use semicolon(;) as a delimiter.</value>
        public virtual string CompanyIds { get; set; }

        /// <summary>
        /// Get the company's id list of company to which this user belongs.
        /// </summary>
        public virtual IReadOnlyList<string> CompanyIdList => CompanyIds.SplitToList(';');

        /// <summary>
        /// Gets or sets a flag indicating if this user is active or not.
        /// </summary>
        /// <value>True if this user is active, otherwise false.</value>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating if this user is super admin or not.
        /// </summary>
        /// <value>True if this user is super admin, otherwise false.</value>
        public bool SuperAdmin { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating if this user is deleted or not.
        /// </summary>
        /// <remarks>
        /// This property is not being used by in the package now. it will be used to support soft delete in future release.
        /// </remarks>
        public bool Deleted { get; set; }

        /// <summary>
        /// Get the list of roles which this user has.
        /// </summary>
        /// <remarks>
        /// <para>Do not set this property to change a user's role. Instead, set <see cref="RoleIds"/>.</para>
        /// </remarks>
        public virtual IReadOnlyList<Role> RoleList { get; set; }

        /// <summary>
        /// Get the list of companys to which this user belongs.
        /// </summary>
        /// <remarks>
        /// <para>Do not set this property to change a user's company. Instead, set <see cref="CompanyIds"/>.</para>
        /// </remarks>
        public virtual IReadOnlyList<Company> CompanyList { get; set; }

        /// <summary>
        /// Gets or sets the name of user who created this user.
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the UTC date that this user is created.
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the name of user who updated this user last time.
        /// </summary>
        public string UpdatedBy { get; set; }

        /// <summary>
        /// Gets or sets the UTC date that this user is updated last time.
        /// </summary>
        public DateTime UpdatedOnUtc { get; set; }
    }
}
