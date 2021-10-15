// Copyright (c) DNV. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNVGL.Authorization.Web.Abstraction
{
    /// <summary>
    /// Provide an abstraction for a storage and management of premissions.
    /// </summary>
    /// <remarks>
    /// <para>Implement this intrefact only if permissions are not defined in source code</para>
    /// <para>Implement <see cref="IPermissionMatrix"/> to define permissions in source code</para>
    /// </remarks>
    public interface IPermissionRepository
    {
        /// <summary>
        /// Return all permissions
        /// </summary>
        /// <returns>A List of <see cref="Task{IEnumerable{PermissionEntity}}"/>.</returns>
        Task<IEnumerable<PermissionEntity>> GetAll();
    }
}
