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
    /// <typeparam name="TUser">The type that represents a user.</typeparam>
    public interface IUser<TUser> where TUser : User
    {
        /// <summary>
        ///  Creates a new user in a store as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user to create in the store.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the user.</returns>
        Task<TUser> Create(TUser user);

        /// <summary>
        /// Get a user which has the specified ID as an asynchronous operation. 
        /// </summary>
        /// <param name="Id">The user ID to look for.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the user.</returns>
        Task<TUser> Read(string Id);

        /// <summary>
        /// Get a user which has the specified Identity ID as an asynchronous operation. 
        /// </summary>
        /// <param name="IdentityId">The identity id to look for. It is an ID provided by Identity provider. It is varacity id if Varacity provide identity.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the user.</returns>
        Task<TUser> ReadByIdentityId(string IdentityId);

        /// <summary>
        /// Updates a user in a store as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user to update in the store.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task Update(TUser user);

        /// <summary>
        /// Deletes a user in a store as an asynchronous operation.
        /// </summary>
        /// <param name="Id">The user ID to delete.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task Delete(string Id);

        /// <summary>
        /// Get a list of all user as an asynchronous operation. 
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> that represents the user list.</returns>
        Task<IEnumerable<TUser>> All();

        /// <summary>
        /// Get a user list of a role
        /// </summary>
        /// <param name="roleId">The role ID to look for.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the user list.</returns>
        Task<IEnumerable<TUser>> GetUsersOfRole(string roleId);

        /// <summary>
        /// Get a user list of a company.
        /// </summary>
        /// <param name="companyId">The company ID to look for.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the user list.</returns>
        Task<IEnumerable<TUser>> GetUsersOfCompany(string companyId);
    }
}
