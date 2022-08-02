using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DNVGL.Authorization.UserManagement.Abstraction;
using DNVGL.Authorization.UserManagement.Abstraction.Entity;
using DNVGL.Authorization.UserManagement.ApiControllers.DTO;
using DNVGL.Authorization.Web;
using DNVGL.Authorization.Web.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static DNVGL.Authorization.Web.PermissionMatrix;

namespace DNVGL.Authorization.UserManagement.ApiControllers
{
    [Authorize]
    [Produces("application/json")]
    [ApiController]
    [TypeFilter(typeof(ErrorCodeExceptionFilter))]
    [Route("api/company/{companyId}/roles")]
    [CompanyIdentityFieldNameFilter(companyIdInRoute: "companyId")]
    [ApiExplorerSettings(GroupName = "UserManagement's Role APIs")]
    public class RolesController<TCompany,TRole,TUser> : UserManagementBaseController<TUser> where TCompany : Company, new() where TRole : Role, new() where TUser : User, new()
    {
        private readonly IRole<TRole> _roleRepository;
        private readonly ICompany<TCompany> _companyRepository;
        private readonly IPermissionRepository _permissionRepository;

        public RolesController(IUser<TUser> userRepository, IRole<TRole> roleRepository, ICompany<TCompany> companyRepository, IPermissionRepository permissionRepository
            , PermissionOptions premissionOptions) : base(userRepository, premissionOptions)
        {
            _roleRepository = roleRepository;
            _companyRepository = companyRepository;
            _permissionRepository = permissionRepository;
        }

        /// <summary>
        /// Get all roles of a company.
        /// </summary>
        /// <remarks>
        /// Required Permission for user in the this company: ViewRole 
        /// 
        /// Required Permission for user not in this company: ViewRole,ViewCompany
        /// </remarks>
        /// <param name="companyId">Company Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        //[Authorize(Roles = "ViewRole")]
        [PermissionAuthorize(Premissions.ViewRole)]
        [AccessCrossCompanyPermissionFilter(Premissions.ViewCompany)]
        [AccessibleCompanyFilter]
        public async Task<IEnumerable<RoleViewDto>> GetCompanyRoles([FromRoute] string companyId)
        {
            return await GetRolesByCompanyId(companyId);
        }

        /// <summary>
        /// Get a role by its id
        /// </summary>
        /// <remarks>
        /// Required Permission for user in the this company: ViewRole 
        /// 
        /// Required Permission for user not in this company: ViewRole,ViewCompany
        /// </remarks>
        /// <param name="companyId">Company Id</param>
        /// <param name="id">Role Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        [PermissionAuthorize(Premissions.ViewRole)]
        [AccessCrossCompanyPermissionFilter(Premissions.ViewCompany)]
        [AccessibleCompanyFilter]
        public async Task<RoleViewDto> GetRole([FromRoute] string companyId,[FromRoute] string id)
        {
            var roles = await GetCompanyRoles(companyId);

            if (roles.Any(t => t.Id == id))
            {
                return await FetchRole(id, _permissionRepository, _roleRepository);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Create a role using custom model. Only if custom role model is used.
        /// </summary>
        /// <remarks>
        /// Required Permission for user in the this company: ManageRole 
        /// 
        /// Required Permission for user not in this company: ManageRole,ViewCompany
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
        /// <param name="companyId">Company Id</param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("custommodel")]
        [PermissionAuthorize(Premissions.ManageRole)]
        [AccessCrossCompanyPermissionFilter(Premissions.ViewCompany)]
        [AccessibleCompanyFilter]
        [ApiExplorerSettings(GroupName = "UserManagement's Role APIs - Custom Model")]
        public async Task<string> CreateRoleFromCustomModel([FromRoute] string companyId, [FromBody] TRole model)
        {
            var user = await GetCurrentUser();
            var permissionKeys = await PrunePermissions(companyId, model.Permissions.SplitToList(';'));
            model.Permissions = permissionKeys.JoinList(";"); 
            model.CreatedBy = $"{user.FirstName} {user.LastName}";
            model.CompanyId = companyId;
            model = await _roleRepository.Create(model);
            return model.Id;
        }

        /// <summary>
        /// Create a role
        /// </summary>
        /// <remarks>
        /// Required Permission for user in the this company: ManageRole 
        /// 
        /// Required Permission for user not in this company: ManageRole,ViewCompany
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
        /// <param name="companyId">Company Id</param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [PermissionAuthorize(Premissions.ManageRole)]
        [AccessCrossCompanyPermissionFilter(Premissions.ViewCompany)]
        [AccessibleCompanyFilter]
        public async Task<string> CreateRole([FromRoute] string companyId,[FromBody] RoleEditModel model)
        {
            var user = await GetCurrentUser();
            var permissionKeys = await PrunePermissions(companyId, model.PermissionKeys);

            var role = new TRole
            {
                Description = model.Description,
                Name = model.Name,
                Active = model.Active,
                CompanyId = companyId,
                Permissions = permissionKeys.JoinList(";"),
                CreatedBy = $"{user.FirstName} {user.LastName}"
            };
            role = await _roleRepository.Create(role);
            return role.Id;
        }

        /// <summary>
        /// Update a role using custom model. Only if custom role model is used.
        /// </summary>
        /// <remarks>
        /// Required Permission for user in the this company: ManageRole 
        /// 
        /// Required Permission for user not in this company: ManageRole,ViewCompany
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
        /// <param name="companyId">Company Id</param>
        /// <param name="id">Role Id</param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("custommodel/{id}")]
        [PermissionAuthorize(Premissions.ManageRole)]
        [ApiExplorerSettings(GroupName = "UserManagement's Role APIs - Custom Model")]
        [AccessCrossCompanyPermissionFilter(Premissions.ViewCompany)]
        [AccessibleCompanyFilter]
        public async Task UpdateRoleFromCustomModel([FromRoute] string companyId, [FromRoute] string id, TRole model)
        {
            var currentUser = await GetCurrentUser();
            var roles = await GetCompanyRoles(companyId);

            if (roles.Any(t => t.Id == id))
            {
                var role = await _roleRepository.Read(id);
                var permissionKeys = await PrunePermissions(model.CompanyId, model.Permissions.SplitToList(';'));
                model.Id = role.Id;
                model.CompanyId = companyId;
                model.Permissions = permissionKeys.JoinList(";");
                model.UpdatedBy = $"{currentUser.FirstName} {currentUser.LastName}";
                await _roleRepository.Update(model);
            }
        }

        /// <summary>
        /// Update a role
        /// </summary>
        /// <remarks>
        /// Required Permission for user in the this company: ManageRole 
        /// 
        /// Required Permission for user not in this company: ManageRole,ViewCompany
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
        /// <param name="companyId">Company Id</param>
        /// <param name="id">Role Id</param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        [PermissionAuthorize(Premissions.ManageRole)]
        [AccessCrossCompanyPermissionFilter(Premissions.ViewCompany)]
        [AccessibleCompanyFilter]
        public async Task UpdateRole([FromRoute] string companyId, [FromRoute] string id, RoleEditModel model)
        {
            var currentUser = await GetCurrentUser();
            var roles = await GetCompanyRoles(companyId);

            if (roles.Any(t => t.Id == id))
            {
                var role = await _roleRepository.Read(id);
                var permissionKeys = await PrunePermissions(companyId, model.PermissionKeys);
                role.Id = id;
                role.Active = model.Active;
                role.Description = model.Description;
                role.Name = model.Name;
                role.CompanyId = companyId;
                role.Permissions = permissionKeys.JoinList(";");
                role.UpdatedBy = $"{currentUser.FirstName} {currentUser.LastName}";
                await _roleRepository.Update(role);
            }
        }

        /// <summary>
        /// Delete Role
        /// </summary>
        /// <remarks>
        /// Required Permission for user in the this company: ManageRole 
        /// 
        /// Required Permission for user not in this company: ManageRole,ViewCompany
        /// </remarks>
        /// <param name="companyId">Company Id</param>
        /// <param name="id">Role Id</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        [PermissionAuthorize(Premissions.ManageRole)]
        [AccessCrossCompanyPermissionFilter(Premissions.ViewCompany)]
        [AccessibleCompanyFilter]
        public async Task DeleteRole([FromRoute] string companyId, [FromRoute] string id)
        {
            var roles = await GetCompanyRoles(companyId);

            if (roles.Any(t => t.Id == id))
            {
                await _roleRepository.Delete(id);
            }

        }

        /// <summary>
        /// Get all roles
        /// </summary>
        /// <remarks>
        /// Required Permission: ViewRole, ViewCompany 
        /// </remarks>
        /// <returns></returns>
        [HttpGet]
        [Route("~/api/roles")]
        [PermissionAuthorize(Premissions.ViewRole, Premissions.ViewCompany)]
        public async Task<IEnumerable<RoleViewDto>> GetRoles()
        {
            return await GetAllRoles(_roleRepository, _permissionRepository);
        }

        [HttpGet]
        [Route("~/api/crosscompany/roles/{companyid}")]
        [PermissionAuthorize(Premissions.ViewRole, Premissions.ViewCompany)]
        [ObsoleteAttribute("It's an obsoleted end point. not suggest to use.", true)]
        public async Task<IEnumerable<RoleViewDto>> GetCrossCompanyRoles([FromRoute] string companyid)
        {
            return await GetRolesByCompanyId(companyid);
        }

        [HttpGet]
        [Route("~/api/crosscompany/roles/{id}")]
        [PermissionAuthorize(Premissions.ViewRole, Premissions.ViewCompany)]
        [ObsoleteAttribute("It's an obsoleted end point. not suggest to use.", true)]
        public async Task<RoleViewDto> GetCrosscompanyRole([FromRoute] string id)
        {
            return await FetchRole(id, _permissionRepository, _roleRepository);
        }


        [HttpPost]
        [Route("~/api/crosscompany/roles/custommodel")]
        [PermissionAuthorize(Premissions.ManageRole, Premissions.ViewCompany)]
        [ApiExplorerSettings(GroupName = "UserManagement's Role APIs - Custom Model")]
        [ObsoleteAttribute("It's an obsoleted end point. not suggest to use.", true)]
        public async Task<string> CreateCrosscompanyRoleFromCustomModel([FromBody] TRole model)
        {
            var permissionKeys = await PrunePermissions(model.CompanyId, model.Permissions.SplitToList(';'));
            var currentUser = await GetCurrentUser();
            model.Permissions = permissionKeys.JoinList(";");
            model.CreatedBy = $"{currentUser.FirstName} {currentUser.LastName}";

            model = await _roleRepository.Create(model);
            return model.Id;
        }


        [HttpPost]
        [Route("~/api/crosscompany/{companyId}/roles")]
        [PermissionAuthorize(Premissions.ManageRole, Premissions.ViewCompany)]
        [ObsoleteAttribute("It's an obsoleted end point. not suggest to use.", true)]
        public async Task<string> CreateCrosscompanyRole([FromRoute] string companyId, [FromBody] RoleEditModel model)
        {
            var permissionKeys = await PrunePermissions(companyId, model.PermissionKeys);
            var currentUser = await GetCurrentUser();

            var role = new TRole
            {
                Description = model.Description,
                Name = model.Name,
                Active = model.Active,
                CompanyId = companyId,
                Permissions = permissionKeys.JoinList(";"),
                CreatedBy = $"{currentUser.FirstName} {currentUser.LastName}",
            };
            role = await _roleRepository.Create(role);
            return role.Id;
        }

        [HttpPut]
        [Route("~/api/crosscompany/roles/custommodel/{id}")]
        [PermissionAuthorize(Premissions.ManageRole, Premissions.ViewCompany)]
        [ApiExplorerSettings(GroupName = "UserManagement's Role APIs - Custom Model")]
        [ObsoleteAttribute("It's an obsoleted end point. not suggest to use.", true)]
        public async Task UpdateCrosscompanyRoleFromCustomModel([FromRoute] string id, TRole model)
        {
            var currentUser = await GetCurrentUser();
            var role = await _roleRepository.Read(id);
            var permissionKeys = await PrunePermissions(model.CompanyId, model.Permissions.SplitToList(';'));
            model.Id = role.Id;
            model.Permissions = permissionKeys.JoinList(";");
            model.UpdatedBy = $"{currentUser.FirstName} {currentUser.LastName}";
            await _roleRepository.Update(model);
        }

        [HttpPut]
        [Route("~/api/crosscompany/{companyId}/roles/{id}")]
        [PermissionAuthorize(Premissions.ManageRole, Premissions.ViewCompany)]
        [ObsoleteAttribute("It's an obsoleted end point. not suggest to use.", true)]
        public async Task UpdateCrosscompanyRole([FromRoute] string id, [FromRoute] string companyId, RoleEditModel model)
        {
            var currentUser = await GetCurrentUser();
            var role = await _roleRepository.Read(id);
            var permissionKeys = await PrunePermissions(companyId, model.PermissionKeys);
            role.Id = id;
            role.Description = model.Description;
            role.Active = model.Active;
            role.Name = model.Name;
            role.Permissions = permissionKeys.JoinList(";");
            role.CompanyId = companyId;
            role.UpdatedBy = $"{currentUser.FirstName} {currentUser.LastName}";
            await _roleRepository.Update(role);
        }

        [HttpDelete]
        [Route("~/api/crosscompany/roles/{id}")]
        [PermissionAuthorize(Premissions.ManageRole, Premissions.ViewCompany)]
        [ObsoleteAttribute("It's an obsoleted end point. not suggest to use.", true)]
        public async Task DeleteCrosscompanyRole([FromRoute] string id)
        {
            await _roleRepository.Delete(id);
        }

        private async Task<IList<string>> PrunePermissions(string companyId, IList<string> sourcePermissionKeys)
        {
            if (sourcePermissionKeys == null || sourcePermissionKeys.Count == 0)
                return null;

            var company = await _companyRepository.Read(companyId);
            return sourcePermissionKeys.Where(t => company.PermissionKeys.Any(f => f == t)).ToList();
        }

        private async Task<IEnumerable<RoleViewDto>> GetRolesByCompanyId(string companyId)
        {
            var roles = await _roleRepository.GetRolesOfCompany(companyId);
            var allPermissions = await _permissionRepository.GetAll();


            var result = roles.Select(t =>
            {
                var dto = t.ToViewDto<RoleViewDto>();

                if (t.PermissionKeys != null)
                {
                    dto.Permissions = allPermissions.Where(p => t.PermissionKeys.Contains(p.Key));
                }

                return dto;
            });

            return result;
        }
    }
}
