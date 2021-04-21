using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DNVGL.Authorization.Web.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace DNVGL.Authorization.Web
{
    internal class PermissionAuthorizationHandler : AttributeAuthorizationHandler<PermissionRequirement, PermissionAuthorizeAttribute>
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserPermissionReader _userPermission;
        private readonly PermissionOptions _premissionOptions;

        public PermissionAuthorizationHandler(
                IHttpContextAccessor httpContextAccessor, IUserPermissionReader userPermission, PermissionOptions permissionOptions)
        {
            _httpContextAccessor = httpContextAccessor;
            _userPermission = userPermission;
            _premissionOptions = permissionOptions;
        }


        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement, IEnumerable<PermissionAuthorizeAttribute> attributes)
        {
            var varacityId = _premissionOptions.GetUserIdentity(_httpContextAccessor.HttpContext);
            var requiredPermissions = attributes.SelectMany(t => t.PermissionsToCheck).ToList();
            var ownedPermissions = (await _userPermission.GetPermissions(varacityId))??new List<PermissionEntity>();

            if (requiredPermissions.Any() == false || requiredPermissions.All(t => ownedPermissions.Any(x => x.Key == t)) || requiredPermissions.All(t => ownedPermissions.Any(x => x.Id == t)))
            {
                context.Succeed(requirement);
            }
            else
            {
                var missedPermissions = requiredPermissions.Where(t => ownedPermissions.Any(x => x.Key == t) == false).ToList();
                _premissionOptions.HandleUnauthorizedAccess(_httpContextAccessor.HttpContext,string.Join(",", missedPermissions));
            }
        }
    }
}
