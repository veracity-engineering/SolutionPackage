// Copyright (c) DNV. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using DNVGL.Authorization.UserManagement.Abstraction.Entity;
using DNVGL.Authorization.Web;

namespace DNVGL.Authorization.UserManagement.ApiControllers.DTO
{
    /// <summary>
    /// Represents company information for a company edit record.
    /// </summary>
    public class CompanyEditModel
    {
        /// <summary>
        /// Gets or sets the company name for this company.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the company description for this company.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the service id for this company.
        /// </summary>
        public string ServiceId { get; set; }

        /// <summary>
        /// Gets or sets the company web domain for this company.
        /// </summary>
        public string DomainUrl { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating if this company is active or not.
        /// </summary>
        /// <value>True if this company is active, otherwise false.</value>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets the company permissions for this company.
        /// </summary>
        public IEnumerable<string> PermissionKeys { get; set; }
    }

    /// <summary>
    /// Represents company information for a company view record.
    /// </summary>
    public class CompanyViewDto : Company
    {
        [JsonIgnore]
        public override string Permissions { get; set; }

        [JsonIgnore]
        public override IReadOnlyList<string> PermissionKeys { get;}

        /// <summary>
        /// Gets or sets the company permissions for this company.
        /// </summary>
        public IEnumerable<PermissionEntity> permissions { get; set; }
    }
}
