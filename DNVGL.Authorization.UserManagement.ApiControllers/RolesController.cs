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
    [Route("api/roles")]
    public class RolesController : ControllerBase
    {
        private readonly IRole _roleRepository;

        public RolesController(IRole roleRepository)
            => _roleRepository = roleRepository;


        [HttpGet]
        [Route("")]
        public async Task<IEnumerable<Role>> GetRoles()
        {
            var result = await _roleRepository.All();

            return result;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<Role> GetRole([FromRoute] string id)
        {
            var result = await _roleRepository.Read(id);
            return result;
        }

        [HttpPost]
        [Route("")]
        public async Task<string> CreateRole([FromBody] RoleEditModel model)
        {
            var role = new Role
            {
                Description = model.Description,
                Name = model.Name,
                Active = model.Active,
                Permissions = string.Join(';', model.PermissionKeys)
            };
            role = await _roleRepository.Create(role);
            return role.Id;
        }

        [HttpPut]
        [Route("{id}")]
        public async Task UpdateRole([FromRoute] string id, RoleEditModel model)
        {
            var role = await _roleRepository.Read(id);
            role.Id = id;
            role.Active = model.Active;
            role.Description = model.Description;
            role.Name = model.Name;
            role.Permissions = string.Join(';', model.PermissionKeys);
            await _roleRepository.Update(role);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task DeleteRole([FromRoute] string id)
        {
            await _roleRepository.Delete(id);
        }
    }
}
