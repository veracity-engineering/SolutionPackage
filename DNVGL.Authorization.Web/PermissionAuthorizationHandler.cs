﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DNVGL.Authorization.Web.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

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
            var httpContext = _httpContextAccessor.HttpContext;
            var varacityId = _premissionOptions.GetUserIdentity(httpContext);
            var companyId = httpContext.GetRouteData().Values["companyId"] as string;

            if (string.IsNullOrEmpty(companyId) && _premissionOptions.GetCompanyIdentity != null)
            {
                companyId = _premissionOptions.GetCompanyIdentity(httpContext);
            }

            var requiredPermissions = attributes.SelectMany(t => t.PermissionsToCheck).ToList();
            var ownedPermissions = (await _userPermission.GetPermissions(varacityId, companyId)) ?? new List<PermissionEntity>();

            if (!requiredPermissions.Any() || requiredPermissions.All(t => ownedPermissions.Any(x => x.Key == t)) || requiredPermissions.All(t => ownedPermissions.Any(x => x.Id == t)))
            {
                context.Succeed(requirement);
            }
            else
            {
                var missedPermissions = requiredPermissions.Where(t => !ownedPermissions.Any(x => x.Key == t)).ToList();
                _premissionOptions.HandleUnauthorizedAccess(_httpContextAccessor.HttpContext, string.Join(",", missedPermissions));
            }
        }
    }
}
