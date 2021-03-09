using System;
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
    [Route("api/roles")]
    public class RolesController : ControllerBase
    {
        private readonly IRole _roleRepository;
        private readonly IPermissionRepository _permissionRepository;

        public RolesController(IRole roleRepository, IPermissionRepository permissionRepository)
        {
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
        }


        [HttpGet]
        [Route("")]
        [PermissionAuthorize(Premissions.ViewRole)]
        public async Task<IEnumerable<RoleViewDto>> GetRoles()
        {
            var roles = await _roleRepository.All();
            var allPermissions = await _permissionRepository.GetAll();


            var result = roles.Select(t =>
            {
                var dto = t.ToViewDto<RoleViewDto>();

                if (t.PermissionKeys != null)
                {
                    dto.Permission = allPermissions.Where(p => t.PermissionKeys.Contains(p.Key));
                }

                return dto;
            });

            return result;
        }

        [HttpGet]
        [Route("{id}")]
        [PermissionAuthorize(Premissions.ViewRole)]
        public async Task<Role> GetRole([FromRoute] string id)
        {
            var role = await _roleRepository.Read(id);
            var allPermissions = await _permissionRepository.GetAll();
            var result = role.ToViewDto<RoleViewDto>();
            result.Permission = allPermissions.Where(p => role.PermissionKeys.Contains(p.Key));

            return result;
        }

        [HttpPost]
        [Route("")]
        [PermissionAuthorize(Premissions.ManageRole)]
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
        [PermissionAuthorize(Premissions.ManageRole)]
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
        [PermissionAuthorize(Premissions.ManageRole)]
        public async Task DeleteRole([FromRoute] string id)
        {
            await _roleRepository.Delete(id);
        }
    }
}
