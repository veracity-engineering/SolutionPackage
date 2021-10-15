// Copyright (c) DNV. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
using System.Threading.Tasks;
using DNVGL.Authorization.UserManagement.Abstraction.Entity;

namespace DNVGL.Authorization.UserManagement.Abstraction
{
    /// <summary>
    /// Provides an abstraction for user synchronization between local and third party platform.
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    public interface IUserSynchronization<TUser> where TUser : User
    {
        /// <summary>
        /// push a user update to the third party platform.
        /// </summary>
        /// <param name="user">The user to update in the third party platform.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the user.</returns>
        Task<TUser> PushUserToSource(TUser user);

        /// <summary>
        /// pull a user update from the third party platform.
        /// </summary>
        /// <param name="user">The user to update at the local.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the user.</returns>
        Task<TUser> PullUserFromSource(TUser user);

        /// <summary>
        /// remove a user or user access on the third party platform.
        /// </summary>
        /// <param name="user">The user to remove at the third party platform.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the user.</returns>
        Task<TUser> RemoveUserAtSource(TUser user);

        /// <summary>
        /// remove a user or user access at the local.
        /// </summary>
        /// <param name="user">The user to remove at the local.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the user.</returns>
        Task<TUser> RemoveUserAtLocal(TUser user);
    }
}
