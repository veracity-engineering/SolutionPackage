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

            return premissions;
        }
    }
}
