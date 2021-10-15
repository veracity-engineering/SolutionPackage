// Copyright (c) DNV. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DNVGL.Authorization.Web
{
    /// <summary>
    /// Provides extension methods to enum.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Get the key of permission from an enum.<b>the enum should be decorated with <see cref="PermissionValueAttribute"/</b>
        /// </summary>
        /// <param name="enumValue">An enum has be decorated with <see cref="PermissionValueAttribute"/></param>
        /// <returns>the permission key.</returns>
        public static string GetPermissionKey(this Enum enumValue)
        {
            FieldInfo fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
            PermissionValueAttribute[] attrs =
                fieldInfo.GetCustomAttributes(typeof(PermissionValueAttribute), false) as PermissionValueAttribute[];

            return attrs.Length > 0 ? attrs[0].Key : string.Empty;
        }

        /// <summary>
        /// Get all refered type of a <see cref="Type"/>
        /// </summary>
        /// <param name="type">The target <see cref="Type"/>.</param>
        /// <returns>The collection of types. <see cref="IEnumerable{Type}"/></returns>
        public static IEnumerable<Type> EnumerateNestedTypes(this Type type)
        {
            const BindingFlags flags = BindingFlags.DeclaredOnly | BindingFlags.Public;
            Queue<Type> toBeVisited = new Queue<Type>();
            toBeVisited.Enqueue(type);
            do
            {
                Type[] nestedTypes = toBeVisited.Dequeue().GetNestedTypes(flags);
                for (int i = 0, l = nestedTypes.Length; i < l; i++)
                {
                    Type t = nestedTypes[i];
                    yield return t;
                    toBeVisited.Enqueue(t);
                }
            } while (toBeVisited.Count != 0);
        }

        /// <summary>
        /// Get a refered type of a <see cref="Type"/> with a predicate function.
        /// </summary>
        /// <param name="type">The target <see cref="Type"/>.</param>
        /// <param name="filter">A predicate to filter type</param>
        /// <returns>The <see cref="Type"/></returns>
        public static Type FindNestedType(this Type type, Predicate<Type> filter)
        {
            const BindingFlags flags = BindingFlags.DeclaredOnly | BindingFlags.Public;
            Type[] nestedTypes = type.GetNestedTypes(flags);
            foreach (var nestedType in nestedTypes)
            {
                if (filter(nestedType))
                {
                    return nestedType;
                }
            }
            foreach (var nestedType in nestedTypes)
            {
                Type result = FindNestedType(nestedType, filter);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }
    }
}
