// Copyright (c) DNV. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;

namespace DNVGL.Authorization.UserManagement.Abstraction.Entity
{
    /// <summary>
    /// Represents company information for a company record.
    /// </summary>
    public class Company
    {
        /// <summary>
        /// Gets or sets the primary key for this company.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the company name for this company.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description name for this company.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the company domain or sub domain name for this company.
        /// </summary>
        public string DomainUrl { get; set; }

        /// <summary>
        /// Gets or sets the company permissions for this company.
        /// </summary>
        /// <value>Permissions are combined as a string which use semicolon(;) as a delimiter.</value>
        public string Permissions { get; set; }

        /// <summary>
        /// Gets the company permission list for this company.
        /// </summary>
        public IReadOnlyList<string> PermissionKeys => (Permissions ?? string.Empty).SplitToList(';');

        /// <summary>
        /// Gets or sets a flag indicating if this company is active or not.
        /// </summary>
        /// <value>True if this company is active, otherwise false.</value>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating if this company is deleted or not.
        /// </summary>
        /// <remarks>
        /// This property is not being used by in the package now. it will be used to support soft delete in future release.
        /// </remarks>
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets the name of user who created this company.
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the UTC date that this company is created.
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the name of user who updated this company last time.
        /// </summary>
        public string UpdatedBy { get; set; }

        /// <summary>
        /// Gets or sets the UTC date that this company is updated.
        /// </summary>
        public DateTime UpdatedOnUtc { get; set; }
    }
}
