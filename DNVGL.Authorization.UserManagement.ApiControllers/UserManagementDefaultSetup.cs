using System;
using System.Collections.Generic;
using System.Text;
using DNVGL.Authorization.UserManagement.Abstraction;
using DNVGL.Authorization.UserManagement.EFCore;
using DNVGL.Authorization.Web;
using DNVGL.Authorization.Web.Abstraction;
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
        public static IServiceCollection AddUserManagement(this IServiceCollection services, Action<DbContextOptionsBuilder> dbContextOptionBuilder, Func<PermissionOptions> buildPermissionOptions = null)
        {
            return services
                .AddDbContext<UserManagementContext>(dbContextOptionBuilder)
                .AddPermissionAuthorization<UserPermissionReader>(buildPermissionOptions)
                .AddScoped<IRole, RoleRepository>()
                .AddScoped<IUser, UserRepository>()
                .AddScoped<ICompany, CompanyRepository>();
        }

    }
}
