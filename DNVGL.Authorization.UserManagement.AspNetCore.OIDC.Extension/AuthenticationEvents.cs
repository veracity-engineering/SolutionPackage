using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using DNVGL.Authorization.Web;
using DNVGL.Authorization.Web.Abstraction;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace DNVGL.Authorization.UserManagement.AspNetCore.OIDC.Extension
{
    public static class AuthenticationEvents
    {
        [ObsoleteAttribute("This function is obsolete. do not use.", true)]
        public static readonly Func<CookieSigningInContext, Task> SigningIn = (ctx) =>
        {
            var identity = ctx.Principal.Identity as ClaimsIdentity;
            var routeClaim = identity.FindFirst("AuthorizationTenantRoute");
            if (routeClaim != null)
                ctx.CookieOptions.Path = routeClaim.Value;

            return Task.CompletedTask;
        };

        [ObsoleteAttribute("This function is obsolete. do not use.", true)]
        public static readonly Func<TokenValidatedContext, IServiceCollection, Task> TokenValidated = async (ctx, services) =>
        {
            var serviceProvider = services.BuildServiceProvider();
            var userPermission = serviceProvider.GetService<IUserPermissionReader>();
            var premissionOptions = serviceProvider.GetService<PermissionOptions>();

            var varacityId = premissionOptions.GetUserIdentity(ctx.Principal);
            var companyId = GetCompanyId(ctx.HttpContext, premissionOptions);

            if (string.IsNullOrEmpty(companyId))
                return;

            var ownedPermissions = (await userPermission.GetPermissions(varacityId, companyId)) ?? new List<PermissionEntity>();

            ctx.Principal.AddIdentity(
                    new ClaimsIdentity(new List<Claim>() {
                            new Claim("AuthorizationTenantRoute", companyId),
                            new Claim("AuthorizationCompanyId", companyId),
                            new Claim(ClaimTypes.Role, string.Join(',',ownedPermissions.Select(t=>t.Key))),
                            new Claim("AuthorizationPermissions", string.Join(',',ownedPermissions.Select(t=>t.Key)))}));

        };


        [ObsoleteAttribute("This function is obsolete. do not use.", true)]
        public static OpenIdConnectEvents AddTokenValidatedHandler(this OpenIdConnectEvents openIdConnectEvents, IServiceCollection services)
        {
            openIdConnectEvents.OnTokenValidated = async ctx =>
            {
                await TokenValidated.Invoke(ctx, services);
            };
            return openIdConnectEvents;

        }


        [ObsoleteAttribute("This function is obsolete. do not use.", true)]
        public static CookieAuthenticationEvents AddSigningInHandler(this CookieAuthenticationEvents cookieEvents, IServiceCollection services)
        {
            cookieEvents.OnSigningIn = SigningIn;
            cookieEvents.OnValidatePrincipal = async ctx =>
            {
                var serviceProvider = services.BuildServiceProvider();
                var userPermission = serviceProvider.GetService<IUserPermissionReader>();
                var premissionOptions = serviceProvider.GetService<PermissionOptions>();
                var companyId = GetCompanyId(ctx.HttpContext, premissionOptions);
                if (!string.IsNullOrEmpty(companyId))
                {
                    var companyIdClaim = ctx.Principal.FindFirst("AuthorizationCompanyId");
                    if (companyIdClaim == null || companyIdClaim.Value!= companyId)
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
            };
            return cookieEvents;
        }

        private static string GetCompanyId(HttpContext context, PermissionOptions premissionOptions)
        {
            var companyId = context.Request.Headers["AUTHORIZATION.COMPANYID"];

            if (string.IsNullOrEmpty(companyId) && premissionOptions.GetCompanyIdentity != null)
            {
                companyId = premissionOptions.GetCompanyIdentity(context);
            }

            if (string.IsNullOrEmpty(companyId))
            {
                companyId = context.GetRouteData().Values["companyId"] as string ?? context.Request.Query["companyId"];
            }

            if (string.IsNullOrEmpty(companyId))
            {
                var action = context.GetEndpoint()?.Metadata?.SingleOrDefault(md => md is ControllerActionDescriptor) as ControllerActionDescriptor;
                CompanyIdentityFieldNameFilterAttribute companyIdentityAttriute = null;
                if (action != null)
                {
                    companyIdentityAttriute = action.ControllerTypeInfo.UnderlyingSystemType.GetCustomAttribute(typeof(CompanyIdentityFieldNameFilterAttribute), true) as CompanyIdentityFieldNameFilterAttribute ?? action.MethodInfo.GetCustomAttribute(typeof(CompanyIdentityFieldNameFilterAttribute), true) as CompanyIdentityFieldNameFilterAttribute;
                    if (companyIdentityAttriute != null)
                    {
                        companyIdentityAttriute.GetCompanyId(context);
                    }
                }
            }

            return companyId;
        }
    }
}
