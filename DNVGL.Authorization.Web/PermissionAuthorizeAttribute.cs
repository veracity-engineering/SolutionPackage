using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace DNVGL.Authorization.Web
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class PermissionAuthorizeAttribute : AuthorizeAttribute
    {
        private string[] _permissionsToCheck;

        public string[] PermissionsToCheck
        {
            get { return _permissionsToCheck; }
        }

        public PermissionAuthorizeAttribute(params string[] permissionsToCheck) : base("PermissionAuthorize")
        {
            _permissionsToCheck = permissionsToCheck;
        }

        public PermissionAuthorizeAttribute(params Enum[] permissionsToCheck) : base("PermissionAuthorize") 
        {
            _permissionsToCheck = permissionsToCheck.Select(x => x.GetPermissionKey()).ToArray();
        }
    }
}
