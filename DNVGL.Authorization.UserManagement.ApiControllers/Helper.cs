using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using DNVGL.Authorization.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;

namespace DNVGL.Authorization.UserManagement.ApiControllers
{
    internal static class Helper
    {
        internal static string GetAccessCrossCompanyPermission(HttpContext context)
        {
            var premissions = context.Request.Headers["AUTHORIZATION.CORSS.COMPANY.PERMISSIONS"];

            //if (string.IsNullOrEmpty(premissions))
            //{
            //    var action = endpoint?.Metadata?.SingleOrDefault(md => md is ControllerActionDescriptor) as ControllerActionDescriptor;
            //    AccessCrossCompanyPermissionFilterAttribute crossCompanyPermissionAttriute = null;
            //    if (action != null)
            //    {
            //        crossCompanyPermissionAttriute = action.ControllerTypeInfo.UnderlyingSystemType.GetCustomAttribute(typeof(AccessCrossCompanyPermissionFilterAttribute), true) as AccessCrossCompanyPermissionFilterAttribute ?? action.MethodInfo.GetCustomAttribute(typeof(AccessCrossCompanyPermissionFilterAttribute), true) as AccessCrossCompanyPermissionFilterAttribute;
            //        if (crossCompanyPermissionAttriute != null && crossCompanyPermissionAttriute.PermissionsToCheck!= null)
            //        {
            //            premissions = string.Join(',', crossCompanyPermissionAttriute.PermissionsToCheck);
            //        }
            //    }
            //}

            return premissions;
        }
    }
}
