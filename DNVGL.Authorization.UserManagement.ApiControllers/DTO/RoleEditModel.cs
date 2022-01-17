// Copyright (c) DNV. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
using System.Collections.Generic;
using DNVGL.Authorization.UserManagement.Abstraction;
using DNVGL.Authorization.UserManagement.Abstraction.Entity;
using DNVGL.Authorization.Web;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace DNVGL.Authorization.UserManagement.ApiControllers.DTO
{
    /// <summary>
    /// Represents role information for a role edit record.
    /// </summary>
    public class RoleEditModel
    {
        /// <summary>
        /// Gets or sets the role name for this role.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the role description for this role.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating if this role is active or not.
        /// </summary>
        /// <value>True if this role is active, otherwise false.</value>
        public bool Active { get; set; }

        /// <summary>
        /// Gets the company permission list for this role.
        /// </summary>
        [Required]
        public IList<string> PermissionKeys { get; set; }
    }

    /// <summary>
    /// Represents role information for a role view record..
    /// </summary>
    public class RoleViewDto : Role
    {
        [JsonIgnore]
        public override string Permissions { get; set; }

        [JsonIgnore]
        public override string CompanyId { get; set; }

        [JsonIgnore]
        public override IReadOnlyList<string> PermissionKeys { get; }

        /// <summary>
        /// Gets the company permission list for this role.
        /// </summary>
        public IEnumerable<PermissionEntity> permissions { get; set; }
    }
}
