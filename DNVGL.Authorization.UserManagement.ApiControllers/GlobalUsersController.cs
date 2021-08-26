using System;
using System.Collections.Generic;
using System.Text;
using DNVGL.Authorization.UserManagement.Abstraction;
using DNVGL.Authorization.UserManagement.Abstraction.Entity;
using DNVGL.Authorization.Web.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DNVGL.Authorization.UserManagement.ApiControllers
{
    [Authorize]
    [ApiController]
    [Route("api/users")]
    [TypeFilter(typeof(ErrorCodeExceptionFilter))]
    [ApiExplorerSettings(GroupName = "UserManagement's User APIs")]
    public class GlobalUsersController<TRole, TUser> : UserManagementBaseController<TUser> where TRole : Role, new() where TUser : User, new()
    {
        private readonly IRole<TRole> _roleRepository;
        private readonly IUser<TUser> _userRepository;
        private readonly PermissionOptions _premissionOptions;
        private readonly IPermissionRepository _permissionRepository;

        public GlobalUsersController(IUser<TUser> userRepository, IRole<TRole> roleRepository, IUserSynchronization<TUser> userSynchronization, PermissionOptions premissionOptions, IPermissionRepository permissionRepository) : base(userRepository, premissionOptions)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _premissionOptions = premissionOptions;
            _permissionRepository = permissionRepository;
        }
    }
}
