// Copyright (c) DNV. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
using System;
using DNVGL.Authorization.UserManagement.Abstraction;
using DNVGL.Authorization.UserManagement.Abstraction.Entity;
using DNVGL.Authorization.Web;
using Microsoft.Extensions.DependencyInjection;

namespace DNVGL.Authorization.UserManagement.ApiControllers
{
    /// <summary>
    /// Provides extension methods to register user management related services to <see cref="IServiceCollection"/>
    /// </summary>
    public static class UserManagementDefaultSetup
    {

        /// <summary>
        /// <para>Configure database connection string for user management module.</para>
        /// <para>Thie extension methods will setup user management module with built in services and data models:<see cref="Company"/>,<see cref="Role"/> and <see cref="User"/>.</para>
        /// <example>
        /// <list type="number">
        /// <item>
        /// Use SQLServer as backend database to store user data.
        /// <code>
        /// services.AddUserManagement(
        ///     new UserManagementOptions{
        ///         DbContextOptionsBuilder = options => options.UseSqlServer(@"Data Source=.\SQLEXPRESS;Initial Catalog=UserManagement;Trusted_Connection=Yes;"),
        /// });
        /// </code>
        /// </item>
        /// <item>
        /// Use Cosmos DB as backend database to store user data.
        /// <code>
        /// services.AddUserManagement(
        ///     new UserManagementOptions{
        ///         DbContextOptionsBuilder = options => options.UseCosmos("https://localhost:8081", "*****", databaseName: "UserManagement"),
        ///         ModelBuilder = (modelBuilder) => modelBuilder.HasDefaultContainer("User"),
        /// });
        /// </code>
        /// </item>
        /// </list>
        /// </example>
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/></param>
        /// <param name="options">A instance of <see cref="UserManagementOptions"/> to configure the user management module.</param>
        /// <returns><see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddUserManagement(this IServiceCollection services, UserManagementOptions options = null)
        {
            return services.AddUserManagement<DummyUserSynchronization>(options ?? new UserManagementOptions());
        }

        private static IServiceCollection AddUserManagement<TUserSynchronization>(this IServiceCollection services, UserManagementOptions options) where TUserSynchronization : IUserSynchronization<User>
        {
            return services.AddUserManagementWithCustomModel<Company, Role, User, TUserSynchronization>(options);
        }

        /// <summary>
        /// <para>Configure database connection string for user management module.</para>
        /// <para>Thie extension methods will setup user management module with built in services and data models:<see cref="Company"/> and <see cref="Role"/>.</para>
        /// <para>A customized user model should be specified to replace generic type: <c>TUser</c></para>
        /// </summary>
        /// <typeparam name="TUser">The type that represents a customized user model.It mush be inherited from <see cref="User"/>.</typeparam>
        /// <param name="services"><see cref="IServiceCollection"/></param>
        /// <param name="options">A instance of <see cref="UserManagementOptions"/> to configure the user management module.</param>
        /// <returns><see cref="IServiceCollection"/></returns>
        /// <remarks>
        /// <example>For example,
        /// <code>
        /// services.AddUserManagementWithCustomModel&lt;User&gt;(
        /// ...
        /// });
        /// </code>
        /// </example>
        /// </remarks>
        public static IServiceCollection AddUserManagementWithCustomModel<TUser>(this IServiceCollection services, UserManagementOptions options) where TUser : User, new()
        {
            return services.AddUserManagementWithCustomModel<Company, Role, TUser, DummyUserSynchronization>(options);
        }

        /// <summary>
        /// <para>Configure database connection string for user management module.</para>
        /// <para>Thie extension methods will setup user management module with built in services and data models:<see cref="Role"/>.</para>
        /// <para>customized company and user model should be specified to replace generic type: <c>TCompany</c>, <c>TUser</c></para>
        /// </summary>
        /// <typeparam name="TCompany">The type that represents a customized company model.It mush be inherited from <see cref="Company"/>.</typeparam>
        /// <typeparam name="TUser">The type that represents a customized user model.It mush be inherited from <see cref="User"/>.</typeparam>
        /// <param name="services"><see cref="IServiceCollection"/></param>
        /// <param name="options">A instance of <see cref="UserManagementOptions"/> to configure the user management module.</param>
        /// <returns><see cref="IServiceCollection"/></returns>
        /// <remarks>
        /// <example>For example,
        /// <code>
        /// services.AddUserManagementWithCustomModel&lt;Company,User&gt;(
        /// ...
        /// });
        /// </code>
        /// </example>
        /// </remarks>
        public static IServiceCollection AddUserManagementWithCustomModel<TCompany, TUser>(this IServiceCollection services, UserManagementOptions options) where TCompany : Company, new() where TUser : User, new()
        {
            return services.AddUserManagementWithCustomModel<TCompany, Role, TUser, DummyUserSynchronization>(options);
        }

        /// <summary>
        /// <para>Configure database connection string for user management module.</para>
        /// <para>Thie extension methods will setup user management module with built in services.</para>
        /// <para>customized company, role and user model should be specified to replace generic type: <c>TCompany</c>, <c>TRole</c>, <c>TUser</c></para>
        /// </summary>
        /// <typeparam name="TCompany">The type that represents a customized company model.It mush be inherited from <see cref="Company"/>.</typeparam>
        /// <typeparam name="TRole">The type that represents a customized role model.It mush be inherited from <see cref="Role"/>.</typeparam>
        /// <typeparam name="TUser">The type that represents a customized user model.It mush be inherited from <see cref="User"/>.</typeparam>
        /// <param name="services"><see cref="IServiceCollection"/></param>
        /// <param name="options">A instance of <see cref="UserManagementOptions"/> to configure the user management module.</param>
        /// <returns><see cref="IServiceCollection"/></returns>
        /// <remarks>
        /// <example>For example,
        /// <code>
        /// services.AddUserManagementWithCustomModel&lt;Company,Role,User&gt;(
        /// ...
        /// });
        /// </code>
        /// </example>
        /// </remarks>
        public static IServiceCollection AddUserManagementWithCustomModel<TCompany, TRole, TUser>(this IServiceCollection services, UserManagementOptions options) where TCompany : Company, new() where TRole : Role, new() where TUser : User, new()
        {
            return services.AddUserManagementWithCustomModel<TCompany, TRole, TUser, DummyUserSynchronization>(options);
        }

        private static IServiceCollection AddUserManagementWithCustomModel<TCompany, TRole, TUser, TUserSynchronization>(this IServiceCollection services, UserManagementOptions options) where TCompany : Company, new() where TRole : Role, new() where TUser : User, new() where TUserSynchronization : IUserSynchronization<User>
        {
            services.AddMvcCore()
              .ConfigureApplicationPartManager(manager =>
              {
                  manager.FeatureProviders.Add(new CustomControllerFeatureProvider(GetValidControllers<TCompany, TRole, TUser>(options.Mode)));
              });

            services.AddSingleton(provider =>
            {
                return new UserManagementSettings
                {
                    Mode = options.Mode
                };
            });

            return services.AddScoped(typeof(IUserSynchronization<TUser>), typeof(TUserSynchronization))
                           .AddPermissionAuthorizationWithoutUserPermissionReader(options.PermissionOptions)
                           .AddScoped<AccessibleCompanyFilterAttribute>()
                           .AddScoped<CompanyIdentityFieldNameFilterAttribute>();
        }

        /// <summary>
        /// <para>Configure database connection string for user management module.</para>
        /// <para>customized data access service to be registered into <see cref="IServiceCollection"/> at other place.</para>
        /// <para>customized company, role and user model should be specified to replace generic type: <c>TCompany</c>, <c>TRole</c>, <c>TUser</c>.</para>
        /// </summary>
        /// <typeparam name="TCompany">The type that represents a customized company model.It mush be inherited from <see cref="Company"/>.</typeparam>
        /// <typeparam name="TRole">The type that represents a customized role model.It mush be inherited from <see cref="Role"/>.</typeparam>
        /// <typeparam name="TUser">The type that represents a customized user model.It mush be inherited from <see cref="User"/>.</typeparam>
        /// <param name="services"><see cref="IServiceCollection"/></param>
        /// <param name="options">A instance of <see cref="UserManagementOptions"/> to configure the user management module.</param>
        /// <returns><see cref="IServiceCollection"/></returns>
        /// <remarks>
        /// <example>For example,
        /// <code>
        /// services.AddUserManagementWithCustomModel&lt;Company,Role,User&gt;(
        /// ...
        /// });
        /// services.AddDbContext&lt;UserManagementContext&gt;(options => options.UseSqlServer(@"Data Source=.\SQLEXPRESS;Initial Catalog=UserManagement;Trusted_Connection=Yes;"))
        ///        .AddScoped&lt;IUserPermissionReader, UserPermissionReader&gt;()
        ///        .AddScoped&lt;IRole&lt;Role&gt;, RoleRepository&gt;()
        ///        .AddScoped&lt;IUser&lt;User&gt;, UserRepository&gt;()
        ///        .AddScoped&lt;ICompany&lt;Company&gt;, CompanyRepository&gt;();
        /// </code>
        /// </example>
        /// </remarks>
        public static IServiceCollection AddUserManagementWithCustomModelAndCRUD<TCompany, TRole, TUser>(this IServiceCollection services, UserManagementOptions options)
            where TCompany : Company, new() where TRole : Role, new() where TUser : User, new()
        {
            return services.AddUserManagementWithCustomModelAndCRUD<TCompany, TRole, TUser, DummyUserSynchronization>(options);
        }

        private static IServiceCollection AddUserManagementWithCustomModelAndCRUD<TCompany, TRole, TUser, TUserSynchronization>(this IServiceCollection services, UserManagementOptions options)
            where TCompany : Company, new() where TRole : Role, new() where TUser : User, new() where TUserSynchronization : IUserSynchronization<User>
        {
            services.AddMvcCore()
              .ConfigureApplicationPartManager(manager =>
              {
                  manager.FeatureProviders.Add(new CustomControllerFeatureProvider(GetValidControllers<TCompany, TRole, TUser>(options.Mode)));
              });

            services.AddSingleton(provider =>
            {
                return new UserManagementSettings
                {
                    Mode = options.Mode
                };
            });

            return services
              .AddPermissionAuthorizationWithoutUserPermissionReader(options.PermissionOptions)
              .AddScoped(typeof(IUserSynchronization<TUser>), typeof(TUserSynchronization))
              .AddScoped<AccessibleCompanyFilterAttribute>()
              .AddScoped<CompanyIdentityFieldNameFilterAttribute>();
        }




        private static Type[] GetValidControllers<TCompany, TRole, TUser>(UserManagementMode mode) where TCompany : Company, new() where TRole : Role, new() where TUser : User, new()
        {
            switch (mode)
            {
                case UserManagementMode.Company_GlobalRole_User:
                    return new Type[] { typeof(CompaniesController<TCompany, TUser>), typeof(GlobalRolesController<TRole, TUser>), typeof(UsersController<TRole, TUser>) };
                case UserManagementMode.Role_User:
                    return new Type[] { typeof(GlobalRolesController<TRole, TUser>), typeof(GlobalUsersController<TUser>) };
                case UserManagementMode.Company_CompanyRole_User:
                default:
                    return new Type[] { typeof(CompaniesController<TCompany, TUser>), typeof(RolesController<TCompany, TRole, TUser>), typeof(UsersController<TRole, TUser>) };
            }
        }

    }
}
