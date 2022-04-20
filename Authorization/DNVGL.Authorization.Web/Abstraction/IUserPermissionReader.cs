// Copyright (c) DNV. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNVGL.Authorization.Web.Abstraction
{
    /// <summary>
    /// Provide an abstraction for a storage and management of users premissions.
    /// </summary>
    /// <remarks>
    /// <para>It must be implemented in the project</para>
    /// </remarks>
    public interface IUserPermissionReader
    {
        /// <summary>
        /// Get a user's all permissions.
        /// </summary>
        /// <param name="identity">The identity id to look for. It could be the ID provided by Identity provider or the primary key for this user in local database.</param>
        /// <returns>A <see cref="Task{IEnumerable{PermissionEntity}}"/> that represents the user's permission list.</returns>
        Task<IEnumerable<PermissionEntity>> GetPermissions(string identity);

        /// <summary>
        /// Get a permission list from a list of permission keys/Ids.
        /// </summary>
        /// <param name="permissions">a list of permission keys/Ids</param>
        /// <returns>A <see cref="Task{IEnumerable{PermissionEntity}}"/> that represents the permission list.</returns>
        Task<IEnumerable<PermissionEntity>> GetPermissions(IEnumerable<string> permissions);

        /// <summary>
        /// Get a user's all permissions of a company.
        /// </summary>
        /// <param name="identity">The identity id to look for. It could be the ID provided by Identity provider or the primary key for this user in local database.</param>
        /// <param name="companyId">The company ID to look for.</param>
        /// <returns>A <see cref="Task{IEnumerable{PermissionEntity}}"/> that represents the user's permission list.</returns>
        Task<IEnumerable<PermissionEntity>> GetPermissions(string identity,string companyId);
    }
}
