﻿using System;
using System.Collections.Generic;
using System.Text;
using DNVGL.Authorization.Web.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;

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
        public static IServiceCollection AddPermissionAuthorization<T>(this IServiceCollection services) where T : IUserPermissionReader
        {
            return services.AddPermissionAuthorization<T, PermissionRepository>();
        }

        /// <summary>
        /// Setup permission authorization with customized implementation of <see cref="IPermissionRepository"/> and  <see cref="IUserPermissionReader"/>.
        /// </summary>
        /// <typeparam name="T">constraints to <see cref="IUserPermissionReader"/></typeparam>
        /// <typeparam name="R">constraints to <see cref="IPermissionRepository"/></typeparam>
        /// <param name="services"><see cref="IServiceCollection"/></param>
        /// <returns><see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddPermissionAuthorization<T,R>(this IServiceCollection services) where T : IUserPermissionReader where R: IPermissionRepository
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton(typeof(IPermissionRepository), typeof(R));
            services.AddScoped(typeof(IUserPermissionReader), typeof(T));
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

            return services.AddAuthorizationCore(config =>
            {
                config.AddPolicy("PermissionAuthorize", policy => policy.AddRequirements(new PermissionRequirement()));
            });

            //return services.AddAuthorization(config =>
            //{
            //    config.AddPolicy("PermissionAuthorize", policy => policy.AddRequirements(new PermissionRequirement()));
            //});

            //return services;
        }

        //public static Action<AuthorizationOptions> ConfigureAuthorization<T>(this IServiceCollection services) where T : IUserPermissionReader
        //{
        //    services.AddPermissionAuthorization<T>();
        //    return config =>
        //    {
        //        config.AddPolicy("PermissionAuthorize", policy => policy.AddRequirements(new PermissionRequirement()));
        //    };
        //}
    }
}
