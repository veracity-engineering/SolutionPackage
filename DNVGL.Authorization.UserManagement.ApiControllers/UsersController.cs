using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DNVGL.Authorization.UserManagement.Abstraction;
using DNVGL.Authorization.UserManagement.Abstraction.Entity;
using DNVGL.Authorization.UserManagement.ApiControllers.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DNVGL.Authorization.UserManagement.ApiControllers
{
    [Authorize]
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IRole _roleRepository;
        private readonly IUser _userRepository;

        public UsersController(IUser userRepository, IRole roleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }


        [HttpGet]
        [Route("")]
        public async Task<IEnumerable<User>> GetRoles()
        {
            var result = await _userRepository.All();

            return result;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<User> GetUser([FromRoute] string id)
        {
            var result = await _userRepository.Read(id);
            return result;
        }

        [HttpGet]
        [Route("{id}/permissions")]
        public async Task<IEnumerable<string>> GetUserPermissions([FromRoute] string id)
        {
            var user = await _userRepository.Read(id);
            var role = await _roleRepository.Read(user.Role.Id);
            return role.PermissionKeys;
        }

        [HttpPost]
        [Route("")]
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
                RoleId = model.RoleId,
                Email = model.Email,
            };
            user = await _userRepository.Create(user);
            return user.Id;
        }

        [HttpPut]
        [Route("{id}")]
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
            user.RoleId = model.RoleId;
            user.Email = model.Email;
            await _userRepository.Update(user);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task DeleteUser([FromRoute] string id)
        {
            await _userRepository.Delete(id);
        }
    }
}
