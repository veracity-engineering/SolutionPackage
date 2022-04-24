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
            var companyId = context.Request.Headers[Constants.AUTHORIZATION_COMPANYID];

            if (string.IsNullOrEmpty(companyId) && premissionOptions.GetCompanyIdentity != null)
            {
                companyId = premissionOptions.GetCompanyIdentity(context);
            }


            if (string.IsNullOrEmpty(companyId))
            {
                companyId = context.GetRouteData().Values[Constants.COMPANYID] as string ?? context.Request.Query[Constants.COMPANYID];
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

            if (string.IsNullOrEmpty(companyId))
            {
                var companyIdClaim = context.User.FindFirst(Constants.AUTHORIZATIONCOMPANYID);
                if (companyIdClaim != null && companyIdClaim.Value != Constants.COMPANY_ROLE_NOT_RELEVANT)
                {
                    companyId = companyIdClaim.Value;
                }
            }

            return companyId;
        }

        internal static string GetCompanyMemberIgnorePermission(HttpContext context, RouteEndpoint endpoint)
        {
            var premissions = context.Request.Headers[Constants.IGNORE_PERMISSIONS];

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
