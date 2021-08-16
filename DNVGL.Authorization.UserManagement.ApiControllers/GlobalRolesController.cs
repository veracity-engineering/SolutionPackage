using System;
using System.Collections.Generic;
using System.Text;
using DNVGL.Authorization.UserManagement.Abstraction;
using DNVGL.Authorization.Web.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DNVGL.Authorization.UserManagement.ApiControllers
{
    [Authorize]
    [ApiController]
    [Route("api/roles")]
    [TypeFilter(typeof(ErrorCodeExceptionFilter))]
    public class GlobalRolesController : UserManagementBaseController
    {
        private readonly IRole _roleRepository;
        private readonly IPermissionRepository _permissionRepository;

        public GlobalRolesController(IUser userRepository, IRole roleRepository,IPermissionRepository permissionRepository, PermissionOptions premissionOptions) : base(userRepository, premissionOptions)
        {
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
        }
    }
}
