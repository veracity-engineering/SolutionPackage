using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DNVGL.Authorization.Web
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class CompanyMemberIgnorePermissionFilterAttribute : TypeFilterAttribute
    {
        private readonly string[] _permissionsToCheck;

        public string[] PermissionsToCheck
        {
            get { return _permissionsToCheck; }
        }

        public CompanyMemberIgnorePermissionFilterAttribute(params string[] permissionsToCheck)
            : base(typeof(CompanyMemberIgnorePermissionFilterImpl))
        {
            _permissionsToCheck = permissionsToCheck;
            Arguments = new object[] { _permissionsToCheck };
        }

        public CompanyMemberIgnorePermissionFilterAttribute(params object[] permissionsToCheck)
            : base(typeof(CompanyMemberIgnorePermissionFilterImpl))
        {
            _permissionsToCheck = permissionsToCheck.Select(x => (x as Enum).GetPermissionKey()).ToArray();
            Arguments = new object[] { _permissionsToCheck };
        }

        private class CompanyMemberIgnorePermissionFilterImpl : IAsyncActionFilter
        {
            private readonly string[] _permissionsToCheck;

            public CompanyMemberIgnorePermissionFilterImpl(string[] permissionsToCheck)
            {
                _permissionsToCheck = permissionsToCheck;
            }


            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                if (_permissionsToCheck!=null && _permissionsToCheck.Length>0)
                {
                    context.HttpContext.Request.Headers.Remove("AUTHORIZATION.COMPANY.IGNORE.PERMISSIONS");
                    context.HttpContext.Request.Headers.Add("AUTHORIZATION.COMPANY.IGNORE.PERMISSIONS", string.Join(',',_permissionsToCheck));
                }

                await next();
            }
        }
    }
}
