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
    [Produces("application/json")]
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

        /// <summary>
        /// Get all roles.
        /// </summary>
        /// <remarks>
        /// Required Permission: ViewRole
        /// </remarks>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [PermissionAuthorize(Premissions.ViewRole)]
        public async Task<IEnumerable<RoleViewDto>> GetRoles()
        {
            return await GetAllRoles(_roleRepository, _permissionRepository);
        }

        /// <summary>
        /// Get role by role id
        /// </summary>
        /// <remarks>
        /// Required Permission: ViewRole
        /// </remarks>
        /// <param name="id">Role id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        [PermissionAuthorize(Premissions.ViewRole)]
        public async Task<RoleViewDto> GetRole([FromRoute] string id)
        {
            return await FetchRole(id, _permissionRepository, _roleRepository);
        }

        /// <summary>
        /// Create a role using custom model. Only if custom role model is used.
        /// </summary>
        /// <remarks>
        /// Required Permission: ManageRole
        ///  
        /// Sample request:
        ///
        ///     {
        ///        "name": "Item1",
        ///        "description": "",
        ///        "active":true,
        ///        "permissionKeys":["ReadWeather","ManageWeather"]
        ///     }
        ///
        /// </remarks>
        /// <param name="model"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Create a role
        /// </summary>
        /// <remarks>
        /// Required Permission: ManageRole
        /// 
        /// Sample request:
        ///
        ///     {
        ///        "name": "Item1",
        ///        "description": "",
        ///        "active":true,
        ///        "permissionKeys":["ReadWeather","ManageWeather"]
        ///     }
        ///
        /// </remarks>
        /// <param name="model"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Update a role using custom model. Only if custom role model is used.
        /// </summary>
        /// <remarks>
        /// Required Permission: ManageRole
        /// 
        /// Sample request:
        ///
        ///     {
        ///        "name": "Item1",
        ///        "description": "",
        ///        "active":true,
        ///        "permissionKeys":["ReadWeather","ManageWeather"]
        ///     }
        ///
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Update a role.
        /// </summary>
        /// <remarks>
        /// Required Permission: ManageRole
        /// 
        /// Sample request:
        ///
        ///     {
        ///        "name": "Item1",
        ///        "description": "",
        ///        "active":true,
        ///        "permissionKeys":["ReadWeather","ManageWeather"]
        ///     }
        ///
        /// </remarks>
        /// <param name="id">Role Id</param>
        /// <param name="model"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Delete a role.
        /// </summary>
        /// <remarks>
        /// Required Permission: ManageRole
        /// </remarks>
        /// <param name="id">Role Id</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        [PermissionAuthorize(Premissions.ManageRole)]
        public async Task DeleteRole([FromRoute] string id)
        {
            await _roleRepository.Delete(id);

        }

    }
}
