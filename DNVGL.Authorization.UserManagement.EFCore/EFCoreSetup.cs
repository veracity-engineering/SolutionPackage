// Copyright (c) DNV. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
using DNVGL.Authorization.UserManagement.Abstraction;
using DNVGL.Authorization.UserManagement.Abstraction.Entity;
using DNVGL.Authorization.Web.Abstraction;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DNVGL.Authorization.UserManagement.EFCore
{
    /// <summary>
    /// Provides extension methods to register user management's database access implementaion to <see cref="IServiceCollection"/>
    /// </summary>
    public static class EFCoreSetup
    {
        /// <summary>
        /// <para>Configure database connection string and other settings for user management module.</para>
        /// <example>
        /// <list type="number">
        /// <item>
        /// Use SQLServer as backend database to store user data.
        /// <code>
        /// services.AddUserManagement().UseEFCore(new EFCoreOptions
        ///{
        ///   DbContextOptionsBuilder = options => options.UseSqlServer(@"Data Source=.\SQLEXPRESS;Initial Catalog=UserManagement;Trusted_Connection=Yes;")
        ///})
        /// </code>
        /// </item>
        /// <item>
        /// Use Cosmos DB as backend database to store user data.
        /// <code>
        /// services.AddUserManagement().UseEFCore(new EFCoreOptions
        ///{
        ///   DbContextOptionsBuilder = options => options.UseCosmos("https://localhost:8081", "*****", databaseName: "UserManagement"),
        ///   ModelBuilder = (modelBuilder) => modelBuilder.HasDefaultContainer("User"),
        ///})
        /// </code>
        /// </item>
        /// </list>
        /// </example>
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/></param>
        /// <param name="options">A instance of <see cref="EFCoreOptions"/> to configure the user management module.</param>
        /// <returns><see cref="IServiceCollection"/></returns>
        public static IServiceCollection UseEFCore(this IServiceCollection services, EFCoreOptions options)
        {
            return services.UseEFCore<Company, Role, User>(options);
        }

        /// <summary>
        /// <para>Configure database connection string and other settings for user management module.</para>
        /// </summary>
        /// <typeparam name="TUser">The type that represents a customized user model.It mush be inherited from <see cref="User"/>.</typeparam>
        /// <param name="services"><see cref="IServiceCollection"/></param>
        /// <param name="options">A instance of <see cref="EFCoreOptions"/> to configure the user management module.</param>
        /// <returns><see cref="IServiceCollection"/></returns>
        public static IServiceCollection UseEFCore<TUser>(this IServiceCollection services, EFCoreOptions options) where TUser : User, new()
        {
            return services.UseEFCore<Company, Role, TUser>(options);
        }

        /// <summary>
        /// <para>Configure database connection string and other settings for user management module.</para>
        /// </summary>
        /// <typeparam name="TCompany">The type that represents a customized company model.It mush be inherited from <see cref="Company"/>.</typeparam>
        /// <typeparam name="TUser">The type that represents a customized user model.It mush be inherited from <see cref="User"/>.</typeparam>
        /// <param name="services"><see cref="IServiceCollection"/></param>
        /// <param name="options">A instance of <see cref="EFCoreOptions"/> to configure the user management module.</param>
        /// <returns><see cref="IServiceCollection"/></returns>
        public static IServiceCollection UseEFCore<TCompany, TUser>(this IServiceCollection services, EFCoreOptions options) where TCompany : Company, new() where TUser : User, new()
        {
            return services.UseEFCore<TCompany, Role, TUser>(options);
        }

        /// <summary>
        /// <para>Configure database connection string and other settings for user management module.</para>
        /// </summary>
        /// <typeparam name="TCompany">The type that represents a customized company model.It mush be inherited from <see cref="Company"/>.</typeparam>
        /// <typeparam name="TRole">The type that represents a customized role model.It mush be inherited from <see cref="Role"/>.</typeparam>
        /// <typeparam name="TUser">The type that represents a customized user model.It mush be inherited from <see cref="User"/>.</typeparam>
        /// <param name="services"><see cref="IServiceCollection"/></param>
        /// <param name="options">A instance of <see cref="EFCoreOptions"/> to configure the user management module.</param>
        /// <returns><see cref="IServiceCollection"/></returns>
        public static IServiceCollection UseEFCore<TCompany, TRole, TUser>(this IServiceCollection services, EFCoreOptions options) where TCompany : Company, new() where TRole : Role, new() where TUser : User, new()
        {
            return services.AddDbContextFactory<UserManagementContext<TCompany, TRole, TUser>>(options.DbContextOptionsBuilder)
                           .AddScoped<UserManagementContext<TCompany, TRole, TUser>>(p =>
                           {
                               var db = p.GetRequiredService<IDbContextFactory<UserManagementContext<TCompany, TRole, TUser>>>().CreateDbContext();
                               db.HardDelete = options.HardDelete;
                               db.PrebuildModel = options.ModelBuilder;
                               return db;
                           })
                           .AddScoped<IUserPermissionReader, UserPermissionReader<TCompany, TRole, TUser>>()
                           .AddScoped<IRole<TRole>, RoleRepository<TCompany, TRole, TUser>>()
                           .AddScoped<IUser<TUser>, UserRepository<TCompany, TRole, TUser>>()
                           .AddScoped<ICompany<TCompany>, CompanyRepository<TCompany, TRole, TUser>>();
        }
    }
}
