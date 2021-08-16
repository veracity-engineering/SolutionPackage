using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DNVGL.Authorization.UserManagement.Abstraction;
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


        public static IServiceCollection AddUserManagement<T>(this IServiceCollection services, UserManagementOptions options) where T : IUserSynchronization
        {
            services.AddMvcCore()
            .ConfigureApplicationPartManager(manager =>
            {
                manager.FeatureProviders.Remove(manager.FeatureProviders.OfType<ControllerFeatureProvider>().FirstOrDefault());

                manager.FeatureProviders.Add(new CustomControllerFeatureProvider(GetInvalidControllers(options.Mode)));
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
                .AddScoped(typeof(IUserSynchronization), typeof(T))
                .AddScoped<IRole, RoleRepository>()
                .AddScoped<IUser, UserRepository>()
                .AddScoped<ICompany, CompanyRepository>()
                .AddScoped<AccessibleCompanyFilterAttribute>()
                .AddScoped<CompanyIdentityFieldNameFilterAttribute>();
        }

        private static Type[] GetInvalidControllers(UserManagementMode mode)
        {
            switch (mode)
            {
                case UserManagementMode.Company_GlobalRole_User:
                    return new Type[] { typeof(CompaniesController), typeof(GlobalUsersController) };
                case UserManagementMode.Role_User:
                    return new Type[] { typeof(CompaniesController), typeof(RolesController) };
                case UserManagementMode.Company_CompanyRole_User:
                default:
                    return new Type[] { typeof(GlobalRolesController), typeof(GlobalUsersController) };
            }
        }

    }
}
