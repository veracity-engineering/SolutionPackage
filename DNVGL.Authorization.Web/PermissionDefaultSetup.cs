// Copyright (c) DNV. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
using System.Collections.Generic;
using DNVGL.Authorization.Web.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http.Features;

namespace DNVGL.Authorization.Web
{
    /// <summary>
    /// Extension methods to register permission related services to <see cref="IServiceCollection"/>
    /// </summary>
    public static class PermissionDefaultSetup
    {


        /// <summary>
        /// Setup permission authorization with default <see cref="PermissionRepository"/> and customized implementation of <see cref="IUserPermissionReader"/>.
        /// <para>Register <see cref="IPermissionRepository"/>'s default implementation <see cref="PermissionRepository"/> which fetch all permissions defined in source code.</para>
        /// <para>The implementation of <see cref="IUserPermissionReader"/> must be specified to replace generic type TUserPermissionReader</para>
        /// </summary>
        /// <typeparam name="TUserPermissionReader">The implemenation of <see cref="IUserPermissionReader"/></typeparam>
        /// <param name="services"><see cref="IServiceCollection"/></param>
        /// <param name="permissionOptions">An optional parameter.<see cref="PermissionOptions"/> controls the permission check behavior.</param>
        /// <returns><see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddPermissionAuthorization<TUserPermissionReader>(this IServiceCollection services, PermissionOptions permissionOptions = null) where TUserPermissionReader : IUserPermissionReader
        {
            return services.AddPermissionAuthorization<TUserPermissionReader, PermissionRepository>(permissionOptions);
        }

        /// <summary>
        /// Setup permission authorization with customized implementation of <see cref="IPermissionRepository"/> and <see cref="IUserPermissionReader"/>.
        /// </summary>
        /// <typeparam name="TUserPermissionReader">The implemenation of <see cref="IUserPermissionReader"/></typeparam>
        /// <typeparam name="TPermissionRepository">The implemenation of <see cref="IPermissionRepository"/></typeparam>
        /// <param name="services"><see cref="IServiceCollection"/></param>
        /// <param name="permissionOptions">An optional parameter.<see cref="PermissionOptions"/> controls the permission check behavior.</param>
        /// <returns><see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddPermissionAuthorization<TUserPermissionReader, TPermissionRepository>(this IServiceCollection services, PermissionOptions permissionOptions = null) where TUserPermissionReader : IUserPermissionReader where TPermissionRepository : IPermissionRepository
        {
            services.AddPermissionAuthorizationWithoutUserPermissionReader<TPermissionRepository>(permissionOptions);
            return services.AddScoped(typeof(IUserPermissionReader), typeof(TUserPermissionReader));
        }

        /// <summary>
        /// Setup permission authorization with default <see cref="PermissionRepository"/>. <b>Additionaly,IUserPermissionReader's implementation has to be registered at other place. </b>
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/></param>
        /// <param name="permissionOptions">An optional parameter.<see cref="PermissionOptions"/> controls the permission check behavior.</param>
        /// <returns><see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddPermissionAuthorizationWithoutUserPermissionReader(this IServiceCollection services, PermissionOptions permissionOptions = null)
        {
            return services.AddPermissionAuthorizationWithoutUserPermissionReader<PermissionRepository>(permissionOptions);
        }

        /// <summary>
        /// Use you own IPermissionRepository implementation to replace default built-in implementation.
        /// </summary>
        /// <typeparam name="TPermissionRepository">The implemenation of <see cref="IPermissionRepository"/></typeparam>
        /// <param name="services"><see cref="IServiceCollection"/></param>
        /// <returns></returns>
        public static IServiceCollection UsePermissionRepository<TPermissionRepository>(this IServiceCollection services) where TPermissionRepository : IPermissionRepository
        {
            var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IPermissionRepository));
            services.Remove(descriptor);
            services.AddScoped(typeof(IPermissionRepository), typeof(TPermissionRepository));
            return services;
        }

        /// <summary>
        /// Setup permission authorization with customized implementation of <see cref="IPermissionRepository"/>. <b>Additionaly,IUserPermissionReader's implementation has to be registered at other place. </b>
        /// </summary>
        /// <typeparam name="TPermissionRepository">The implemenation of <see cref="IPermissionRepository"/></typeparam>
        /// <param name="services"><see cref="IServiceCollection"/></param>
        /// <param name="permissionOptions">An optional parameter.<see cref="PermissionOptions"/> controls the permission check behavior.</param>
        /// <returns><see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddPermissionAuthorizationWithoutUserPermissionReader<TPermissionRepository>(this IServiceCollection services, PermissionOptions permissionOptions = null) where TPermissionRepository : IPermissionRepository
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<PermissionOptions>(provider =>
            {
                if (permissionOptions == null)
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


            services.AddSingleton(typeof(IPermissionRepository), typeof(TPermissionRepository));
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

            return services.AddAuthorizationCore(config =>
            {
                config.AddPolicy("PermissionAuthorize", policy => policy.AddRequirements(new PermissionRequirement()));
            });
        }

        /// <summary>
        /// Add customized CookieValidateHandler to the <see cref="CookieAuthenticationEvents.OnValidatePrincipal"/>.
        /// <example>
        /// <code>
        /// services.AddAuthentication().AddCookie(o => o.Events.AddCookieValidateHandler(services));
        /// </code>
        /// </example>
        /// </summary>
        /// <remarks>
        /// Claim based authorization is enabled only if this customized CookieValidateHandler are added.
        /// </remarks>
        /// <param name="cookieEvents"><see cref="CookieAuthenticationEvents"/></param>
        /// <param name="services"><see cref="IServiceCollection"/></param>
        /// <returns><see cref="CookieAuthenticationEvents"/></returns>
        public static CookieAuthenticationEvents AddCookieValidateHandler(this CookieAuthenticationEvents cookieEvents, IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var userPermission = serviceProvider.GetService<IUserPermissionReader>();
            var premissionOptions = serviceProvider.GetService<PermissionOptions>();
            return AddCookieValidateHandler(cookieEvents, userPermission, premissionOptions);
        }

        internal static CookieAuthenticationEvents AddCookieValidateHandler(this CookieAuthenticationEvents cookieEvents, IUserPermissionReader userPermission, PermissionOptions premissionOptions)
        {
            var previousValidatePrincipal = cookieEvents.OnValidatePrincipal;

            cookieEvents.OnValidatePrincipal = async ctx =>
            {

                if (previousValidatePrincipal != null)
                {
                    _ = previousValidatePrincipal.Invoke(ctx);
                }

                var endpoint = ctx.HttpContext.Features.Get<IEndpointFeature>()?.Endpoint as RouteEndpoint;
                var companyId = Helper.GetCompanyId(ctx.HttpContext, premissionOptions, endpoint);
                if (!string.IsNullOrEmpty(companyId))
                {
                    var companyIdClaim = ctx.Principal.FindFirst("AuthorizationCompanyId");
                    if (companyIdClaim == null || companyIdClaim.Value != companyId)
                    {
                        var varacityId = premissionOptions.GetUserIdentity(ctx.Principal);
                        var ownedPermissions = (await userPermission.GetPermissions(varacityId, companyId)) ?? new List<PermissionEntity>();

                        var identity = ctx.Principal.Identity as ClaimsIdentity;
                        if (companyIdClaim != null)
                        {
                            ctx.Principal.Claims.Where(t => t.Type == "AuthorizationTenantRoute" 
                            || t.Type == "AuthorizationCompanyId" 
                            || t.Type == "AuthorizationPermissions" 
                            || t.Type== ClaimTypes.Role).ToList().ForEach(t => identity.RemoveClaim(t));
                        }

                        var claims = new List<Claim>() {
                            new Claim("AuthorizationTenantRoute", companyId),
                            new Claim("AuthorizationCompanyId", companyId),
                            new Claim("AuthorizationPermissions", string.Join(',',ownedPermissions.Select(t=>t.Key)))};
                        ownedPermissions.Select(t => t.Key).ToList().ForEach(t => claims.Add(new Claim(ClaimTypes.Role, t)));

                        identity.AddClaims(claims);
                        ctx.ShouldRenew = true;
                    }
                }
            };
            return cookieEvents;
        }
    }
}
