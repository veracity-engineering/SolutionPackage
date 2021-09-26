// Copyright (c) DNV. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
namespace DNVGL.Authorization.Web
{
    /// <summary>
    /// Represents permission information for a permission record.
    /// </summary>
    public class PermissionEntity
    {
        /// <summary>
        /// Gets or sets the primary key for this permission.
        /// </summary>
        /// <remarks>
        /// Permission's Id should be unique at the global level.        
        /// </remarks>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique key for this permission.
        /// </summary>
        /// <remarks>
        /// <para>Permission's Key should be unique at the global level. </para> 
        /// <para>It's better to have a human readable key.</para>
        /// </remarks>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the name for this permission.
        /// </summary>
        /// <remarks>
        /// <para>Display on UI</para>
        /// </remarks>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description for this permission.
        /// </summary>
        /// <remarks>
        /// <para>Display on UI</para>
        /// </remarks>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the group for this permission.
        /// </summary>
        /// <remarks>
        /// <para>group permissions on UI</para>
        /// </remarks>
        public string Group { get; set; }
    }
}
