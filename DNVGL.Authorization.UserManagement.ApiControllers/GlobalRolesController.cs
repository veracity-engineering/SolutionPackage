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
    [TypeFilter(typeof(ErrorCodeExceptionFilter))]
    [ApiExplorerSettings(GroupName = "UserManagement's Role APIs")]
    public class GlobalRolesController<TRole, TUser> : UserManagementBaseController<TUser> where TRole : Role, new() where TUser : User, new()
    {
        private readonly IRole<TRole> _roleRepository;
        private readonly IPermissionRepository _permissionRepository;

        public GlobalRolesController(IUser<TUser> userRepository, IRole<TRole> roleRepository,IPermissionRepository permissionRepository, PermissionOptions premissionOptions) : base(userRepository, premissionOptions)
        {
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
        }

        [HttpGet]
        [Route("")]
        [PermissionAuthorize(Premissions.ViewRole)]
        public async Task<IEnumerable<RoleViewDto>> GetRoles()
        {
            return await GetAllRoles(_roleRepository, _permissionRepository);
        }


        [HttpGet]
        [Route("{id}")]
        [PermissionAuthorize(Premissions.ViewRole)]
        public async Task<RoleViewDto> GetRole([FromRoute] string id)
        {
            return await FetchRole(id, _permissionRepository, _roleRepository);
        }

        [HttpPost]
        [Route("custommodel")]
        [PermissionAuthorize(Premissions.ManageRole)]
        [ApiExplorerSettings(GroupName = "UserManagement's Role APIs - Custom Model")]
        public async Task<string> CreateRoleFromCustomModel([FromBody]TRole model)
        {
            var user = await GetCurrentUser();
            model.CreatedBy = $"{user.FirstName} {user.LastName}";
            model = await _roleRepository.Create(model);
            return model.Id;
        }

        [HttpPost]
        [Route("")]
        [PermissionAuthorize(Premissions.ManageRole)]
        public async Task<string> CreateRole([FromBody] RoleEditModel model)
        {
            var user = await GetCurrentUser();

            var role = new TRole
            {
                Description = model.Description,
                Name = model.Name,
                Active = model.Active,
                Permissions = string.Join(';', model.PermissionKeys),
                CreatedBy = $"{user.FirstName} {user.LastName}"
            };
            role = await _roleRepository.Create(role);
            return role.Id;
        }

        [HttpPut]
        [Route("custommodel/{id}")]
        [PermissionAuthorize(Premissions.ManageRole)]
        [ApiExplorerSettings(GroupName = "UserManagement's Role APIs - Custom Model")]
        public async Task UpdateRoleFromCustomModel([FromRoute] string id, TRole model)
        {
            var currentUser = await GetCurrentUser();
            model.Id = id;
            model.UpdatedBy = $"{currentUser.FirstName} {currentUser.LastName}";
            await _roleRepository.Update(model);
        }


        [HttpPut]
        [Route("{id}")]
        [PermissionAuthorize(Premissions.ManageRole)]
        public async Task UpdateRole([FromRoute] string id, RoleEditModel model)
        {
            var currentUser = await GetCurrentUser();
            var role = await _roleRepository.Read(id);
            role.Id = id;
            role.Active = model.Active;
            role.Description = model.Description;
            role.Name = model.Name;
            role.Permissions = string.Join(';', model.PermissionKeys);
            role.UpdatedBy = $"{currentUser.FirstName} {currentUser.LastName}";
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
