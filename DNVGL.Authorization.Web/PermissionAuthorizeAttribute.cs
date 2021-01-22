using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace DNVGL.Authorization.Web
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class PermissionAuthorizeAttribute : AuthorizeAttribute
    {
        private string[] _permissionsToCheck;

        public string[] PermissionsToCheck
        {
            get { return _permissionsToCheck; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="permissionsToCheck"></param>
        public PermissionAuthorizeAttribute(params string[] permissionsToCheck) : base("PermissionAuthorize")
        {
            _permissionsToCheck = permissionsToCheck;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="permissionsToCheck"></param>
        public PermissionAuthorizeAttribute(params object[] permissionsToCheck) : base("PermissionAuthorize") 
        {
            _permissionsToCheck = permissionsToCheck.Select(x => (x as Enum).GetPermissionKey()).ToArray();
        }
    }
}
