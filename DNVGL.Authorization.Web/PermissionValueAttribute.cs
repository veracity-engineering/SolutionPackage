﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace DNVGL.Authorization.Web
{
    /// <summary>
    /// This attribute is attached to an enum filed to define a permission entity.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class PermissionValueAttribute : Attribute
    {
        /// <summary>
        /// Permission's Id should be unique at global level. 1-4 are reserved Id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// <para>Permission's Key should be unique at global level. It represents a permission. </para> 
        /// <para>The best practice is to assign an understandable value to a key</para>
        /// <para>Be careful to change the key of a permission which is in use in any live environment. </para> 
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
