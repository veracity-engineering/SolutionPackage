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
    public class UsersController : ControllerBase
    {
        private readonly IRole _roleRepository;
        private readonly IUser _userRepository;
        private readonly IUserSynchronization _serSynchronization;
        private readonly PermissionOptions _premissionOptions;

        public UsersController(IUser userRepository, IRole roleRepository, IUserSynchronization userSynchronization,PermissionOptions premissionOptions)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _serSynchronization = userSynchronization;
            _premissionOptions = premissionOptions;
        }


        [HttpGet]
        [Route("")]
        [PermissionAuthorize(Premissions.ViewUser)]
        public async Task<IEnumerable<User>> GetUsers()
        {
            var result = await _userRepository.All();

            return result;
        }

        [HttpGet]
        [Route("{id}")]
        [PermissionAuthorize(Premissions.ViewUser)]
        public async Task<User> GetUser([FromRoute] string id)
        {
            var result = await _userRepository.Read(id);
            return result;
        }

        [HttpGet]
        [Route("currentUser")]
        [PermissionAuthorize(Premissions.ViewUser)]
        public async Task<User> GetUserByIdentityId()
        {
            var varacityId = _premissionOptions.GetUserIdentity(HttpContext);
            var result = await _userRepository.ReadByIdentityId(varacityId);
            return result;
        }


        [HttpGet]
        [Route("{id}/permissions")]
        [PermissionAuthorize(Premissions.ViewUser)]
        public async Task<IEnumerable<string>> GetUserPermissions([FromRoute] string id)
        {
            var user = await _userRepository.Read(id);
            return user.Roles.SelectMany(t => t.PermissionKeys);
        }

        [HttpPost]
        [Route("")]
        [PermissionAuthorize(Premissions.ManageUser)]
        public async Task<string> CreateUser([FromBody] UserEditModel model)
        {
            var user = new User
            {
                Description = model.Description,
                FirstName = model.FirstName,
                LastName = model.LastName,
                VeracityId = model.VeracityId,
                Active = model.Active,
                CompanyId = model.CompanyId,
                RoleIds = string.Join(';', model.RoleIdList),
                Email = model.Email,
            };
            user = await _userRepository.Create(user);
            return user.Id;
        }

        [HttpPut]
        [Route("{id}")]
        [PermissionAuthorize(Premissions.ManageUser)]
        public async Task UpdateUser([FromRoute] string id, UserEditModel model)
        {
            var user = await _userRepository.Read(id);
            user.Id = id;
            user.Active = model.Active;
            user.Description = model.Description;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.VeracityId = model.VeracityId;
            user.CompanyId = model.CompanyId;
            user.RoleIds = string.Join(';', model.RoleIdList);
            user.Email = model.Email;
            await _userRepository.Update(user);
        }

        [HttpPut]
        [Route("sync/{id}")]
        [PermissionAuthorize(Premissions.ManageUser)]
        public async Task SyncUser([FromRoute] string id, UserEditModel model)
        {
            var user = await _userRepository.Read(id);

            await _serSynchronization.SyncUser(user);
        }


        [HttpDelete]
        [Route("{id}")]
        [PermissionAuthorize(Premissions.ManageUser)]
        public async Task DeleteUser([FromRoute] string id)
        {
            await _userRepository.Delete(id);
        }
    }
}
