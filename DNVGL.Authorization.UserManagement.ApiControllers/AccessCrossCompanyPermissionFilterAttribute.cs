using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DNVGL.Authorization.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DNVGL.Authorization.UserManagement.ApiControllers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class AccessCrossCompanyPermissionFilterAttribute : TypeFilterAttribute
    {
        private readonly string[] _permissionsToCheck;

        public string[] PermissionsToCheck
        {
            get { return _permissionsToCheck; }
        }

        public AccessCrossCompanyPermissionFilterAttribute(params string[] permissionsToCheck)
            : base(typeof(AccessCrossCompanyPermissionFilterImpl))
        {
            Order = 2;
            _permissionsToCheck = permissionsToCheck;
            Arguments = new object[] { _permissionsToCheck };
        }

        public AccessCrossCompanyPermissionFilterAttribute(params object[] permissionsToCheck)
            : base(typeof(AccessCrossCompanyPermissionFilterImpl))
        {
            Order = 2;
            _permissionsToCheck = permissionsToCheck.Select(x => (x as Enum).GetPermissionKey()).ToArray();
            Arguments = new object[] { _permissionsToCheck };
        }

        private class AccessCrossCompanyPermissionFilterImpl : IAsyncActionFilter
        {
            private readonly string[] _permissionsToCheck;

            public AccessCrossCompanyPermissionFilterImpl(string[] permissionsToCheck)
            {
                _permissionsToCheck = permissionsToCheck;
            }


            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                if (_permissionsToCheck != null && _permissionsToCheck.Length > 0)
                {
                    context.HttpContext.Request.Headers.Remove("AUTHORIZATION.CORSS.COMPANY.PERMISSIONS");
                    context.HttpContext.Request.Headers.Add("AUTHORIZATION.CORSS.COMPANY.PERMISSIONS", string.Join(',', _permissionsToCheck));
                }

                await next();
            }
        }


    }
}
