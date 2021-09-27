// Copyright (c) DNV. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
using System.Collections.Generic;
using DNVGL.Authorization.UserManagement.Abstraction.Entity;

namespace DNVGL.Authorization.UserManagement.ApiControllers.DTO
{
    /// <summary>
    /// Represents user information for a user edit record.
    /// </summary>
    public class UserEditModel
    {
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
        /// Gets or sets a flag indicating if this user is super admin or not.
        /// </summary>
        /// <value>True if this user is super admin, otherwise false.</value>
        public bool SuperAdmin { get; set; }

        /// <summary>
        /// Gets or sets id of roles which this user has.
        /// </summary>
        /// <value>Role's ids are combined as a string which use semicolon(;) as a delimiter.</value>
        public IList<string> RoleIds { get; set; }

        /// <summary>
        /// Gets or sets id of company to which this user belongs.
        /// </summary>
        /// <value>Company's ids are combined as a string which use semicolon(;) as a delimiter.</value>
        public IList<string> CompanyIds { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating if this user is active or not.
        /// </summary>
        /// <value>True if this user is active, otherwise false.</value>
        public bool Active { get; set; }
    }

    /// <summary>
    /// Represents user information for a user view record.
    /// </summary>
    public class UserViewModel:User
    {
        private new string CompanyIds { get; set; }
        private new string RoleIds { get; set; }
        private new IReadOnlyList<string> RoleIdList { get; set; }
        private new IReadOnlyList<string> CompanyIdList { get; set; }
        private new IReadOnlyList<Role> RoleList { get; set; }
        private new IReadOnlyList<Company> CompanyList { get; set; }

        /// <summary>
        /// Get the list of roles which this user has.
        /// </summary>
        /// <remarks>
        /// <para>Do not set this property to change a user's role. Instead, set <see cref="RoleIds"/>.</para>
        /// </remarks>
        public IEnumerable<RoleViewDto> Roles { get; set; }

        /// <summary>
        /// Get the list of companys to which this user belongs.
        /// </summary>
        /// <remarks>
        /// <para>Do not set this property to change a user's company. Instead, set <see cref="CompanyIds"/>.</para>
        /// </remarks>
        public IEnumerable<CompanyViewDto> Companies { get; set; }

    }
}
