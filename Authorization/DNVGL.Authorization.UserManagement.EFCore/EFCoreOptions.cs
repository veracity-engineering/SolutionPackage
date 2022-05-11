﻿// Copyright (c) DNV. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
using System;
using Microsoft.EntityFrameworkCore;

namespace DNVGL.Authorization.UserManagement.EFCore
{
    /// <summary>
    /// Provides an option to configure the user management EFCore module.
    /// </summary>
    public class EFCoreOptions
    {
        /// <summary>
        /// Gets or sets the action to build Database Context Options.
        /// <example>For example:
        /// <code>
        ///    DbContextOptionsBuilder = options => options.UseSqlServer(@"Data Source=.\SQLEXPRESS;Initial Catalog=UserManagement;Trusted_Connection=Yes;"),
        /// </code>
        /// </example>
        /// </summary>
        public Action<DbContextOptionsBuilder> DbContextOptionsBuilder { get; set; }

        /// <summary>
        /// Gets or sets the action to apply customized module builder logic.
        /// <example>For example, specify the container name in Azure Cosmos DB:
        /// <code>
        ///    ModelBuilder = (modelBuilder) => modelBuilder.HasDefaultContainer("User"),
        /// </code>
        /// </example>
        /// </summary>
        public Action<ModelBuilder> ModelBuilder { get; set; }

        /// <summary>
        /// Gets or sets the behavior of entity deletion. Set true if soft delete is not a desired behavior.
        /// </summary>
        public bool HardDelete { get; set; }
    }
}
