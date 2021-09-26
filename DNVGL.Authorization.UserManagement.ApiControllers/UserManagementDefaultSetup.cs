using System;
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

        public static IServiceCollection AddUserManagement<T>(this IServiceCollection services, UserManagementOptions options) where T : IUserSynchronization<User>
        {
            return services.AddUserManagementWithCustomModel<Company, Role, User, DummyUserSynchronization>(options);
        }

        public static IServiceCollection AddUserManagementWithCustomModel<TUser>(this IServiceCollection services, UserManagementOptions options) where TUser : User, new()
        {
            return services.AddUserManagementWithCustomModel<Company, Role, TUser, DummyUserSynchronization>(options);
        }

        public static IServiceCollection AddUserManagementWithCustomModel<TCompany, TUser>(this IServiceCollection services, UserManagementOptions options) where TCompany : Company, new() where TUser : User, new()
        {
            return services.AddUserManagementWithCustomModel<TCompany, Role, TUser, DummyUserSynchronization>(options);
        }

        public static IServiceCollection AddUserManagementWithCustomModel<TCompany, TRole, TUser>(this IServiceCollection services, UserManagementOptions options) where TCompany : Company, new() where TRole : Role, new() where TUser : User, new()
        {
            return services.AddUserManagementWithCustomModel<TCompany, TRole, TUser, DummyUserSynchronization>(options);
        }

        public static IServiceCollection AddUserManagementWithCustomModel<TCompany, TRole, TUser, TUserSynchronization>(this IServiceCollection services, UserManagementOptions options) where TCompany : Company, new() where TRole : Role, new() where TUser : User, new() where TUserSynchronization : IUserSynchronization<User>
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
                           .AddDbContextFactory<UserManagementContext<TCompany, TRole, TUser>>(options.DbContextOptionsBuilder)
                           .AddScoped<UserManagementContext<TCompany, TRole, TUser>>(p =>
                           {
                               var db = p.GetRequiredService<IDbContextFactory<UserManagementContext<TCompany, TRole, TUser>>>().CreateDbContext();
                               db.PrebuildModel = options.ModelBuilder;
                               return db;
                           })
                           .AddPermissionAuthorization<UserPermissionReader<TCompany, TRole, TUser>>(options.PermissionOptions)
                           .AddScoped(typeof(IUserSynchronization<TUser>), typeof(TUserSynchronization))
                           .AddScoped<IRole<TRole>, RoleRepository<TCompany, TRole, TUser>>()
                           .AddScoped<IUser<TUser>, UserRepository<TCompany, TRole, TUser>>()
                           .AddScoped<ICompany<TCompany>, CompanyRepository<TCompany, TRole, TUser>>()
                           .AddScoped<AccessibleCompanyFilterAttribute>()
                           .AddScoped<CompanyIdentityFieldNameFilterAttribute>();
        }


        public static IServiceCollection AddUserManagementWithCustomModelAndCRUD<TCompany, TRole, TUser>(this IServiceCollection services, UserManagementOptions options)
            where TCompany : Company, new() where TRole : Role, new() where TUser : User, new()
        {
            return services.AddUserManagementWithCustomModelAndCRUD<TCompany, TRole, TUser, DummyUserSynchronization>(options);
        }

        public static IServiceCollection AddUserManagementWithCustomModelAndCRUD<TCompany, TRole, TUser, TUserSynchronization>(this IServiceCollection services, UserManagementOptions options)
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
