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
    /// 
    /// </summary>
    public static class EFCoreSetup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection UseEFCore(this IServiceCollection services, EFCoreOptions options)
        {
            return services.UseEFCore<Company, Role, User>(options);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TUser"></typeparam>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection UseEFCore<TUser>(this IServiceCollection services, EFCoreOptions options) where TUser : User, new()
        {
            return services.UseEFCore<Company, Role, TUser>(options);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TCompany"></typeparam>
        /// <typeparam name="TUser"></typeparam>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection UseEFCore<TCompany, TUser>(this IServiceCollection services, EFCoreOptions options) where TCompany : Company, new() where TUser : User, new()
        {
            return services.UseEFCore<TCompany, Role, TUser>(options);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TCompany"></typeparam>
        /// <typeparam name="TRole"></typeparam>
        /// <typeparam name="TUser"></typeparam>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection UseEFCore<TCompany, TRole, TUser>(this IServiceCollection services, EFCoreOptions options) where TCompany : Company, new() where TRole : Role, new() where TUser : User, new()
        {
            return services.AddDbContextFactory<UserManagementContext<TCompany, TRole, TUser>>(options.DbContextOptionsBuilder)
                           .AddScoped<UserManagementContext<TCompany, TRole, TUser>>(p =>
                           {
                               var db = p.GetRequiredService<IDbContextFactory<UserManagementContext<TCompany, TRole, TUser>>>().CreateDbContext();
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
