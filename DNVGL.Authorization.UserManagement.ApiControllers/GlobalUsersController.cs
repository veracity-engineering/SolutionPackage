﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DNVGL.Authorization.UserManagement.Abstraction;
using DNVGL.Authorization.UserManagement.Abstraction.Entity;
using DNVGL.Authorization.UserManagement.ApiControllers.DTO;
using DNVGL.Authorization.Web;
using DNVGL.Authorization.Web.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static DNVGL.Authorization.Web.PermissionMatrix;

namespace DNVGL.Authorization.UserManagement.ApiControllers
{
    [Authorize]
    [ApiController]
    [Route("api/users")]
    [TypeFilter(typeof(ErrorCodeExceptionFilter))]
    [ApiExplorerSettings(GroupName = "UserManagement's User APIs")]
    public class GlobalUsersController<TUser> : UserManagementBaseController<TUser> where TUser : User, new()
    {
        private readonly IUser<TUser> _userRepository;
        private readonly PermissionOptions _premissionOptions;
        private readonly IPermissionRepository _permissionRepository;

        public GlobalUsersController(IUser<TUser> userRepository, IUserSynchronization<TUser> userSynchronization, PermissionOptions premissionOptions, IPermissionRepository permissionRepository) : base(userRepository, premissionOptions)
        {
            _userRepository = userRepository;
            _premissionOptions = premissionOptions;
            _permissionRepository = permissionRepository;
        }

        [HttpGet]
        [Route("")]
        [PermissionAuthorize(Premissions.ViewUser)]
        public async Task<IEnumerable<UserViewModel>> GetUsers()
        {
            return await GetAllUsers(_userRepository, _permissionRepository);
        }

        [HttpGet]
        [Route("{id}")]
        [PermissionAuthorize(Premissions.ViewUser)]
        public async Task<UserViewModel> GetUser([FromRoute] string id)
        {
            var user = await _userRepository.Read(id);
            return await PopulateUserInfo(user);
        }

        [HttpPut]
        [Route("custommodel/{id}")]
        [PermissionAuthorize(Premissions.ManageUser)]
        [ApiExplorerSettings(GroupName = "UserManagement's User APIs - Custom Model")]
        public async Task UpdateUserFromCustomModel([FromRoute] string id, TUser model)
        {
            var currentUser = await GetCurrentUser();
            model.Id = id;
            model.UpdatedBy = $"{currentUser.FirstName} {currentUser.LastName}";
            await _userRepository.Update(model);
        }


        [HttpPut]
        [Route("{id}")]
        [PermissionAuthorize(Premissions.ManageUser)]
        public async Task UpdateUser([FromRoute] string id, UserEditModel model)
        {
            var currentUser = await GetCurrentUser();
            var user = await _userRepository.Read(id);
            user.Id = id;
            user.Active = model.Active;
            user.Description = model.Description;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.VeracityId = model.VeracityId;
            user.Email = model.Email;
            user.UpdatedBy = $"{currentUser.FirstName} {currentUser.LastName}";
            user.RoleIds = string.Join(';', model.RoleIds);
            await _userRepository.Update(user);
        }

        [HttpPost]
        [Route("custommodel")]
        [PermissionAuthorize(Premissions.ManageUser)]
        [ApiExplorerSettings(GroupName = "UserManagement's User APIs - Custom Model")]
        public async Task<string> CreateUserFromCustommodel([FromBody] TUser model)
        {
            var currentUser = await GetCurrentUser();
            model.CreatedBy = $"{currentUser.FirstName} {currentUser.LastName}";
            model = await _userRepository.Create(model);
            return model.Id;
        }

        [HttpPost]
        [Route("")]
        [PermissionAuthorize(Premissions.ManageUser)]
        public async Task<string> CreateUser([FromBody] UserEditModel model)
        {
            var currentUser = await GetCurrentUser();
            var user = new TUser
            {
                Description = model.Description,
                FirstName = model.FirstName,
                LastName = model.LastName,
                VeracityId = model.VeracityId,
                Active = model.Active,
                RoleIds = string.Join(';', model.RoleIds),
                Email = model.Email,
                CreatedBy = $"{currentUser.FirstName} {currentUser.LastName}",
            };

            user = await _userRepository.Create(user);
            return user.Id;
        }

        [HttpDelete]
        [Route("{id}")]
        [PermissionAuthorize(Premissions.ManageUser)]
        public async Task DeleteUser([FromRoute] string id)
        {
            await _userRepository.Delete(id);
        }


        [HttpGet]
        [Route("~/api/users/currentUser")]
        public async Task<UserViewModel> GetUserByIdentityId()
        {
            var varacityId = _premissionOptions.GetUserIdentity(HttpContext.User);
            var user = await _userRepository.ReadByIdentityId(varacityId);
            return await PopulateUserInfo(user);
        }

        [HttpGet]
        [Route("~/api/users/{id}/permissions")]
        [PermissionAuthorize(Premissions.ViewUser)]
        public async Task<IEnumerable<string>> GetUserCorssCompanyPermissions([FromRoute] string id)
        {
            var user = await _userRepository.Read(id);
            return user.RoleList.SelectMany(t => t.PermissionKeys);
        }



        private async Task<UserViewModel> PopulateUserInfo(TUser user)
        {
            var allPermissions = await _permissionRepository.GetAll();
            var result = user.ToViewDto<UserViewModel>();
            result = PopulateUserRoleInfo(user, result, allPermissions);
            return result;
        }

    }
}
