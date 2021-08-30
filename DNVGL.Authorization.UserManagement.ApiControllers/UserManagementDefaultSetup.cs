﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DNVGL.Authorization.UserManagement.Abstraction;
using DNVGL.Authorization.UserManagement.Abstraction.Entity;
using DNVGL.Authorization.UserManagement.EFCore;
using DNVGL.Authorization.Web;
using DNVGL.Authorization.Web.Abstraction;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DNVGL.Authorization.UserManagement.ApiControllers
{
    public static class UserManagementDefaultSetup
    {
        /// <summary>
        /// To DO
        /// <example>
        /// Demonstrate to configure different database for user management.
        /// <list type="number">
        /// <item>
        /// Use SQLServer as backend database to store user data.
        /// <code>
        /// <para/>services.AddUserManagement(b => b.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFTestSample;ConnectRetryCount=0"));
        /// </code>
        /// </item>
        /// </list>
        /// </example>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="dbContextOptionBuilder"></param>
        /// <returns></returns>
        public static IServiceCollection AddUserManagement(this IServiceCollection services, UserManagementOptions options)
        {
            return services.AddUserManagement<DummyUserSynchronization>(options);
        }

        public static IServiceCollection AddUserManagementWithCustomModelOrCRUD<TCompany, TRole, TUser>(this IServiceCollection services, UserManagementOptions options)
            where TCompany : Company, new() where TRole : Role, new() where TUser : User, new()
        {
            return services.AddUserManagementWithCustomModelOrCRUD<TCompany, TRole, TUser, DummyUserSynchronization>(options);
        }

        public static IServiceCollection AddUserManagementWithCustomModelOrCRUD<TCompany, TRole, TUser, TUserSynchronization>(this IServiceCollection services, UserManagementOptions options)
            where TCompany : Company, new() where TRole : Role, new() where TUser : User, new() where TUserSynchronization : IUserSynchronization<User>
        {
            services.AddMvcCore()
              .ConfigureApplicationPartManager(manager =>
              {
                  manager.FeatureProviders.Add(new CustomControllerFeatureProvider(GetValidControllers<TCompany, TRole, TUser>(options.Mode)));
              });

            return services
              .AddPermissionAuthorizationWithoutUserPermissionReader(options.PermissionOptions)
              .AddScoped(typeof(IUserSynchronization<TUser>), typeof(TUserSynchronization))
              .AddScoped<AccessibleCompanyFilterAttribute>()
              .AddScoped<CompanyIdentityFieldNameFilterAttribute>();
        }


        public static IServiceCollection AddUserManagement<T>(this IServiceCollection services, UserManagementOptions options) where T : IUserSynchronization<User>
        {
            services.AddMvcCore()
            .ConfigureApplicationPartManager(manager =>
            {
                manager.FeatureProviders.Add(new CustomControllerFeatureProvider(GetValidControllers<Company, Role, User>(options.Mode)));
            });

            return services
                .AddDbContextFactory<UserManagementContext>(options.DbContextOptionsBuilder)
                .AddScoped<UserManagementContext>(p =>
                {
                    var db = p.GetRequiredService<IDbContextFactory<UserManagementContext>>().CreateDbContext();
                    db.PrebuildModel = options.ModelBuilder;
                    return db;
                })
                .AddPermissionAuthorization<UserPermissionReader>(options.PermissionOptions)
                .AddScoped(typeof(IUserSynchronization<User>), typeof(T))
                .AddScoped<IRole<Role>, RoleRepository>()
                .AddScoped<IUser<User>, UserRepository>()
                .AddScoped<ICompany<Company>, CompanyRepository>()
                .AddScoped<AccessibleCompanyFilterAttribute>()
                .AddScoped<CompanyIdentityFieldNameFilterAttribute>();
        }

        private static Type[] GetValidControllers<TCompany, TRole, TUser>(UserManagementMode mode) where TCompany : Company, new() where TRole : Role, new() where TUser : User, new()
        {
            switch (mode)
            {
                case UserManagementMode.Company_GlobalRole_User:
                    return new Type[] { typeof(CompaniesController<TCompany, TUser>), typeof(GlobalRolesController<TRole, TUser>), typeof(GlobalUsersController<TRole, TUser>) };
                case UserManagementMode.Role_User:
                    return new Type[] { typeof(GlobalRolesController<TRole, TUser>), typeof(GlobalUsersController<TRole, TUser>) };
                case UserManagementMode.Company_CompanyRole_User:
                default:
                    return new Type[] { typeof(CompaniesController<TCompany, TUser>), typeof(RolesController<TCompany, TRole, TUser>), typeof(UsersController<TRole, TUser>) };
            }
        }

    }
}
