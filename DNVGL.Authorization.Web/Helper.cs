using DNVGL.Authorization.Web.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using System.Linq;
using System.Reflection;

namespace DNVGL.Authorization.Web
{
    internal static class Helper
    {
        internal static string GetCompanyId(HttpContext context, PermissionOptions premissionOptions, RouteEndpoint endpoint)
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
                var action = endpoint?.Metadata?.SingleOrDefault(md => md is ControllerActionDescriptor) as ControllerActionDescriptor;
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

        internal static string GetCompanyMemberIgnorePermission(HttpContext context, RouteEndpoint endpoint)
        {
            var premissions = context.Request.Headers["AUTHORIZATION.COMPANY.IGNORE.PERMISSIONS"];

            if (string.IsNullOrEmpty(premissions))
            {
                var action = endpoint?.Metadata?.SingleOrDefault(md => md is ControllerActionDescriptor) as ControllerActionDescriptor;
                CompanyMemberIgnorePermissionFilterAttribute crossCompanyPermissionAttriute = null;
                if (action != null)
                {
                    crossCompanyPermissionAttriute = action.ControllerTypeInfo.UnderlyingSystemType.GetCustomAttribute(typeof(CompanyMemberIgnorePermissionFilterAttribute), true) as CompanyMemberIgnorePermissionFilterAttribute ?? action.MethodInfo.GetCustomAttribute(typeof(CompanyMemberIgnorePermissionFilterAttribute), true) as CompanyMemberIgnorePermissionFilterAttribute;
                    if (crossCompanyPermissionAttriute != null && crossCompanyPermissionAttriute.PermissionsToIgore != null)
                    {
                        premissions = string.Join(',', crossCompanyPermissionAttriute.PermissionsToIgore);
                    }
                }
            }

            return premissions;
        }

    }
}
