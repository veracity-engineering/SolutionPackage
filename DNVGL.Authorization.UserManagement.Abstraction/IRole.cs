// Copyright (c) DNV. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
using System.Collections.Generic;
using System.Threading.Tasks;
using DNVGL.Authorization.UserManagement.Abstraction.Entity;

namespace DNVGL.Authorization.UserManagement.Abstraction
{
    /// <summary>
    /// Provides an abstraction for a storage and management of roles.
    /// </summary>
    /// <typeparam name="TRole">The type that represents a role.</typeparam>
    public interface IRole<TRole> where TRole : Role
    {
        /// <summary>
        ///  Creates a new role in a store as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role to create in the store.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the role.</returns>
        Task<TRole> Create(TRole role);

        /// <summary>
        /// Get a role which has the specified ID as an asynchronous operation. 
        /// </summary>
        /// <param name="Id">The role ID to look for.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the role.</returns>
        Task<TRole> Read(string Id);

        /// <summary>
        /// Updates a role in a store as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role to update in the store.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task Update(TRole role);

        /// <summary>
        /// Deletes a role in a store as an asynchronous operation.
        /// </summary>
        /// <param name="Id">The role ID to delete.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task Delete(string Id);

        /// <summary>
        /// Get a list of a company's roles as an asynchronous operation. 
        /// </summary>
        /// <param name="companyId">The company ID to look for.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the role list.</returns>
        Task<IEnumerable<TRole>> GetRolesOfCompany(string companyId);

        /// <summary>
        /// Get a list of all role as an asynchronous operation. 
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> that represents the role list.</returns>
        Task<IEnumerable<TRole>> All();
    }
}
