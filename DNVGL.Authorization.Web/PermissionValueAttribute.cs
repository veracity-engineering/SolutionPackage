using System;
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
        /// Construct a permission entity
        /// </summary>
        /// <param name="id">Permission's Id should be unique at global level. 1-4 are reserved Id.</param>
        /// <param name="key">
        /// <para>Permission's Key should be unique at global level. It represents a permission. </para> 
        /// <para>The best practice is to assign an understandable value to a key</para>
        /// <para>Be careful to change the key of a permission which is in use in any live environment. </para> 
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
