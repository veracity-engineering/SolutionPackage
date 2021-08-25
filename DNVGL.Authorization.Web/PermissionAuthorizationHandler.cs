using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DNVGL.Authorization.Web.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
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
            var varacityId = _premissionOptions.GetUserIdentity(httpContext.User);

            var companyId = Helper.GetCompanyId(httpContext, _premissionOptions,context.Resource as RouteEndpoint);

            var requiredPermissions = attributes.SelectMany(t => t.PermissionsToCheck).ToList();
            var ownedPermissionsInClaim = httpContext.User.Claims.FirstOrDefault(t => t.Type == "AuthorizationPermissions")?.Value;
            IEnumerable<PermissionEntity> ownedPermissions = new List<PermissionEntity>();
            if (!string.IsNullOrEmpty(ownedPermissionsInClaim))
            {
                ownedPermissions = await _userPermission.GetPermissions(ownedPermissionsInClaim.Split(',').ToList());
            }
            else
            {
                ownedPermissions = (await _userPermission.GetPermissions(varacityId, companyId)) ?? ownedPermissions;
            }

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

        //private string GetCompanyId(HttpContext context, AuthorizationHandlerContext authContext)
        //{
        //    var companyId = context.Request.Headers["AUTHORIZATION.COMPANYID"];

        //    if (string.IsNullOrEmpty(companyId) && _premissionOptions.GetCompanyIdentity != null)
        //    {
        //        companyId = _premissionOptions.GetCompanyIdentity(context);
        //    }

        //    if (string.IsNullOrEmpty(companyId))
        //    {
        //        companyId = context.GetRouteData().Values["companyId"] as string ?? context.Request.Query["companyId"];
        //    }

        //    if (string.IsNullOrEmpty(companyId))
        //    {
        //        var httpContext = _httpContextAccessor.HttpContext;
        //        var endpoint = authContext.Resource as RouteEndpoint;
        //        var action = endpoint?.Metadata?.SingleOrDefault(md => md is ControllerActionDescriptor) as ControllerActionDescriptor;
        //        CompanyIdentityFieldNameFilterAttribute companyIdentityAttriute = null;
        //        if (action != null)
        //        {
        //            companyIdentityAttriute = action.ControllerTypeInfo.UnderlyingSystemType.GetCustomAttribute(typeof(CompanyIdentityFieldNameFilterAttribute), true) as CompanyIdentityFieldNameFilterAttribute ?? action.MethodInfo.GetCustomAttribute(typeof(CompanyIdentityFieldNameFilterAttribute), true) as CompanyIdentityFieldNameFilterAttribute;
        //            if (companyIdentityAttriute != null)
        //            {
        //                companyIdentityAttriute.GetCompanyId(httpContext);
        //            }
        //        }
        //    }

        //    return companyId;
        //}

    }
}
