// Copyright (c) DNV. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using DNVGL.Authorization.UserManagement.Abstraction.Entity;

namespace DNVGL.Authorization.UserManagement.Abstraction
{
    /// <summary>
    /// Provides an abstraction for a storage and management of companies.
    /// </summary>
    /// <typeparam name="TCompany">The type that represents a company.</typeparam>
    public interface ICompany<TCompany> where TCompany : Company
    {
        /// <summary>
        /// Creates a new company in a store as an asynchronous operation.
        /// </summary>
        /// <param name="company">The company to create in the store.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the company.</returns>
        Task<TCompany> Create(TCompany company);

        /// <summary>
        /// Get a company which has the specified ID as an asynchronous operation. 
        /// </summary>
        /// <param name="Id">The company ID to look for.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the company.</returns>
        Task<TCompany> Read(string Id);

        /// <summary>
        /// Get a company which has the specified web domain as an asynchronous operation. 
        /// </summary>
        /// <param name="domain">The web domain to look for.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the company.</returns>
        Task<TCompany> ReadByDomain(string domain);

        /// <summary>
        /// Get a list of companies which has the specified ID as an asynchronous operation.
        /// </summary>
        /// <param name="Ids">The company ID list to look for.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the company list.</returns>
        Task<IEnumerable<TCompany>> List(IEnumerable<string> Ids);

        /// <summary>
        /// Updates a company in a store as an asynchronous operation.
        /// </summary>
        /// <param name="company">The company to update in the store.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task Update(TCompany company);

        /// <summary>
        /// Deletes a company in a store as an asynchronous operation.
        /// </summary>
        /// <param name="Id">The company ID to delete.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task Delete(string Id);

        /// <summary>
        /// Get a list of all company as an asynchronous operation. 
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> that represents the company list.</returns>
        Task<IEnumerable<TCompany>> All();

        /// <summary>
		/// Return a querable company dataset.
        /// </summary>
        /// <returns></returns>
        IQueryable<TCompany> QueryCompanys();
    }
}
