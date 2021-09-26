// Copyright (c) DNV. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.;
using System;

namespace DNVGL.Authorization.Web
{
    /// <summary>
    /// Represents permission information for a permission record.
    /// </summary>
    /// <remarks>
    /// This attribute is attached to an enum filed to define a permission entity.
    ///</remarks>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class PermissionValueAttribute : Attribute
    {

        /// <summary>
        /// Construct a permission entity
        /// </summary>
        /// <param name="id">Permission's Id should be unique at the global level.</param>
        /// <param name="key">
        /// <para>Permission's Key should be unique at the global level. </para> 
        /// <para>It's better to have a human readable key.</para>
        /// </param>
        /// <param name="name">        
        /// <para>Permission's name.</para>
        /// <para>Display on UI</para></param>
        /// <param name="description">
        /// <para>Permission's description.</para>
        /// <para>Display on UI</para>
        /// </param>
        /// <param name="group">
        /// <para>Permission's group.</para>
        /// <para>group permissions on UI</para>
        ///</param>
        public PermissionValueAttribute(string id, string key, string name, string description, string group)
        {
            Id = id;
            Key = key;
            Name = name;
            Description = description;
            Group = group;
        }
        /// <summary>
        /// Permission's Id should be unique at global level.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// <para>Permission's Key should be unique at the global level. </para> 
        /// <para>It's better to have a human readable key.</para>
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// <para>Permission's name.</para>
        /// <para>Display on UI</para>
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// <para>Permission's description.</para>
        /// <para>Display on UI</para>
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// <para>Permission's group.</para>
        /// <para>group permissions on UI</para>
        /// </summary>
        public string Group { get; set; }

    }
}
