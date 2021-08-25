using System;
using System.Collections.Generic;
using System.Text;
using DNVGL.Authorization.Web.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http.Features;

namespace DNVGL.Authorization.Web
{
    /// <summary>
    /// Extension methods to register permission related service to <see cref="IServiceCollection"/>
    /// </summary>
    public static class PermissionDefaultSetup
    {


        /// <summary>
        /// Setup permission authorization with default <see cref="PermissionRepository"/> and customized implementation of <see cref="IUserPermissionReader"/>.
        /// <para>Register <see cref="IPermissionRepository"/>'s default implementation <see cref="PermissionRepository"/> which fetch all permissions defined in source code.</para>
        /// <para>The implementation of <see cref="IUserPermissionReader"/> must be specified to replace generic type T</para>
        /// </summary>
        /// <typeparam name="T">constraints to <see cref="IUserPermissionReader"/></typeparam>
        /// <param name="services"><see cref="IServiceCollection"/></param>
        /// <returns><see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddPermissionAuthorization<T>(this IServiceCollection services, PermissionOptions permissionOptions = null) where T : IUserPermissionReader
        {
            return services.AddPermissionAuthorization<T, PermissionRepository>(permissionOptions);
        }

        /// <summary>
        /// Setup permission authorization with customized implementation of <see cref="IPermissionRepository"/> and  <see cref="IUserPermissionReader"/>.
        /// </summary>
        /// <typeparam name="T">constraints to <see cref="IUserPermissionReader"/></typeparam>
        /// <typeparam name="R">constraints to <see cref="IPermissionRepository"/></typeparam>
        /// <param name="services"><see cref="IServiceCollection"/></param>
        /// <returns><see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddPermissionAuthorization<T, R>(this IServiceCollection services, PermissionOptions permissionOptions = null) where T : IUserPermissionReader where R : IPermissionRepository
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddScoped<PermissionOptions>(provider =>
            {
                if(permissionOptions == null)
                {
                    permissionOptions = new PermissionOptions();
                }
                if (permissionOptions.GetUserIdentity == null)
                {
                    permissionOptions.GetUserIdentity = (user) => user.Claims.FirstOrDefault(t => t.Type == "userId")?.Value;
                }

                if (permissionOptions.HandleUnauthorizedAccess == null)
                {
                    permissionOptions.HandleUnauthorizedAccess = BuiltinUnauthorizedAccessHandler.Return403ForbiddenCode;
                }

                return permissionOptions;
            });


            services.AddSingleton(typeof(IPermissionRepository), typeof(R));
            services.AddScoped(typeof(IUserPermissionReader), typeof(T));
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

            return services.AddAuthorizationCore(config =>
            {
                config.AddPolicy("PermissionAuthorize", policy => policy.AddRequirements(new PermissionRequirement()));
            });
        }

        public static CookieAuthenticationEvents AddSigningInHandler(this CookieAuthenticationEvents cookieEvents, IServiceCollection services)
        {
            //cookieEvents.OnSigningIn = SigningIn;
            cookieEvents.OnValidatePrincipal = async ctx =>
            {
                var serviceProvider = services.BuildServiceProvider();
                var userPermission = serviceProvider.GetService<IUserPermissionReader>();
                var premissionOptions = serviceProvider.GetService<PermissionOptions>();
                var endpoint = ctx.HttpContext.Features.Get<IEndpointFeature>()?.Endpoint as RouteEndpoint;
                var companyId = Helper.GetCompanyId(ctx.HttpContext, premissionOptions, endpoint);
                if (!string.IsNullOrEmpty(companyId))
                {
                    var companyIdClaim = ctx.Principal.FindFirst("AuthorizationCompanyId");
                    if (companyIdClaim == null || companyIdClaim.Value != companyId)
                    {
                        var varacityId = premissionOptions.GetUserIdentity(ctx.Principal);
                        var ownedPermissions = (await userPermission.GetPermissions(varacityId, companyId)) ?? new List<PermissionEntity>();
                        ctx.Principal.AddIdentity(
                        new ClaimsIdentity(new List<Claim>() {
                            new Claim("AuthorizationTenantRoute", companyId),
                            new Claim("AuthorizationCompanyId", companyId),
                            new Claim(ClaimTypes.Role, string.Join(',',ownedPermissions.Select(t=>t.Key))),
                            new Claim("AuthorizationPermissions", string.Join(',',ownedPermissions.Select(t=>t.Key)))}));
                        ctx.ReplacePrincipal(ctx.Principal);
                        ctx.ShouldRenew = true;
                    }
                }
                return;
            };
            return cookieEvents;
        }

    }
}
