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
    [ApiController]
    [TypeFilter(typeof(ErrorCodeExceptionFilter))]
    [Route("api/mycompany/{companyId}/roles")]
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


        [HttpGet]
        [Route("")]
        //[Authorize(Roles = "ViewRole")]
        [PermissionAuthorize(Premissions.ViewRole)]
        public async Task<IEnumerable<RoleViewDto>> GetCompanyRoles([FromRoute] string companyId)
        {
            return await GetRolesByCompanyId(companyId);
        }

        [HttpGet]
        [Route("{id}")]
        [PermissionAuthorize(Premissions.ViewRole)]
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

        [HttpPost]
        [Route("custommodel")]
        [PermissionAuthorize(Premissions.ManageRole)]
        [ApiExplorerSettings(GroupName = "UserManagement's Role APIs - Custom Model")]
        public async Task<string> CreateRoleFromCustomModel([FromRoute] string companyId, [FromBody] TRole model)
        {
            var user = await GetCurrentUser();
            var permissionKeys = await PrunePermissions(companyId, model.Permissions.SplitToList(';'));
            model.Permissions = string.Join(';', permissionKeys);
            model.CreatedBy = $"{user.FirstName} {user.LastName}";
            model.CompanyId = companyId;
            model = await _roleRepository.Create(model);
            return model.Id;
        }

        [HttpPost]
        [Route("")]
        [PermissionAuthorize(Premissions.ManageRole)]
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
                Permissions = string.Join(';', permissionKeys),
                CreatedBy = $"{user.FirstName} {user.LastName}"
            };
            role = await _roleRepository.Create(role);
            return role.Id;
        }


        [HttpPut]
        [Route("custommodel/{id}")]
        [PermissionAuthorize(Premissions.ManageRole)]
        [ApiExplorerSettings(GroupName = "UserManagement's Role APIs - Custom Model")]
        public async Task UpdateRoleFromCustomModel([FromRoute] string companyId, [FromRoute] string id, TRole model)
        {
            var currentUser = await GetCurrentUser();
            var roles = await GetCompanyRoles(companyId);

            if (roles.Any(t => t.Id == id))
            {
                var role = await _roleRepository.Read(id);
                var permissionKeys = await PrunePermissions(model.CompanyId, model.Permissions.SplitToList(';'));
                model.Id = role.Id;
                model.CompanyId = model.CompanyId;
                model.Permissions = string.Join(';', permissionKeys);
                model.UpdatedBy = $"{currentUser.FirstName} {currentUser.LastName}";
                await _roleRepository.Update(model);
            }
        }


        [HttpPut]
        [Route("{id}")]
        [PermissionAuthorize(Premissions.ManageRole)]
        public async Task UpdateRole([FromRoute] string companyId, [FromRoute] string id, RoleEditModel model)
        {
            var currentUser = await GetCurrentUser();
            var roles = await GetCompanyRoles(companyId);

            if (roles.Any(t => t.Id == id))
            {
                var role = await _roleRepository.Read(id);
                var permissionKeys = await PrunePermissions(model.CompanyId, model.PermissionKeys);
                role.Id = id;
                role.Active = model.Active;
                role.Description = model.Description;
                role.Name = model.Name;
                role.CompanyId = model.CompanyId;
                role.Permissions = string.Join(';', permissionKeys);
                role.UpdatedBy = $"{currentUser.FirstName} {currentUser.LastName}";
                await _roleRepository.Update(role);
            }
        }

        [HttpDelete]
        [Route("{id}")]
        [PermissionAuthorize(Premissions.ManageRole)]
        public async Task DeleteRole([FromRoute] string companyId, [FromRoute] string id)
        {
            var roles = await GetCompanyRoles(companyId);

            if (roles.Any(t => t.Id == id))
            {
                await _roleRepository.Delete(id);
            }

        }

        [HttpGet]
        [Route("~/api/crosscompany/roles")]
        [PermissionAuthorize(Premissions.ViewRole, Premissions.ViewCompany)]
        public async Task<IEnumerable<RoleViewDto>> GetRoles()
        {
            var roles = await _roleRepository.All();
            var allPermissions = await _permissionRepository.GetAll();


            var result = roles.Select(t =>
            {
                var dto = t.ToViewDto<RoleViewDto>();

                if (t.PermissionKeys != null)
                {
                    dto.permissions = allPermissions.Where(p => t.PermissionKeys.Contains(p.Key));
                }

                return dto;
            });

            return result;
        }

        [HttpGet]
        [Route("~/api/crosscompany/roles/{companyid}")]
        [PermissionAuthorize(Premissions.ViewRole, Premissions.ViewCompany)]
        public async Task<IEnumerable<RoleViewDto>> GetCrossCompanyRoles([FromRoute] string companyid)
        {
            return await GetRolesByCompanyId(companyid);
        }

        [HttpGet]
        [Route("~/api/crosscompany/roles/{id}")]
        [PermissionAuthorize(Premissions.ViewRole, Premissions.ViewCompany)]
        public async Task<RoleViewDto> GetCrosscompanyRole([FromRoute] string id)
        {
            return await FetchRole(id, _permissionRepository, _roleRepository);
        }


        [HttpPost]
        [Route("~/api/crosscompany/roles/custommodel")]
        [PermissionAuthorize(Premissions.ManageRole, Premissions.ViewCompany)]
        [ApiExplorerSettings(GroupName = "UserManagement's Role APIs - Custom Model")]
        public async Task<string> CreateCrosscompanyRoleFromCustomModel([FromBody] TRole model)
        {
            var permissionKeys = await PrunePermissions(model.CompanyId, model.Permissions.SplitToList(';'));
            var currentUser = await GetCurrentUser();
            model.Permissions = string.Join(';', permissionKeys);
            model.CreatedBy = $"{currentUser.FirstName} {currentUser.LastName}";

            model = await _roleRepository.Create(model);
            return model.Id;
        }


        [HttpPost]
        [Route("~/api/crosscompany/roles")]
        [PermissionAuthorize(Premissions.ManageRole, Premissions.ViewCompany)]
        public async Task<string> CreateCrosscompanyRole([FromBody] RoleEditModel model)
        {
            var permissionKeys = await PrunePermissions(model.CompanyId, model.PermissionKeys);
            var currentUser = await GetCurrentUser();

            var role = new TRole
            {
                Description = model.Description,
                Name = model.Name,
                Active = model.Active,
                CompanyId = model.CompanyId,
                Permissions = string.Join(';', permissionKeys),
                CreatedBy = $"{currentUser.FirstName} {currentUser.LastName}",
            };
            role = await _roleRepository.Create(role);
            return role.Id;
        }

        [HttpPut]
        [Route("~/api/crosscompany/roles/custommodel/{id}")]
        [PermissionAuthorize(Premissions.ManageRole, Premissions.ViewCompany)]
        [ApiExplorerSettings(GroupName = "UserManagement's Role APIs - Custom Model")]
        public async Task UpdateCrosscompanyRoleFromCustomModel([FromRoute] string id, TRole model)
        {
            var currentUser = await GetCurrentUser();
            var role = await _roleRepository.Read(id);
            var permissionKeys = await PrunePermissions(model.CompanyId, model.Permissions.SplitToList(';'));
            model.Id = role.Id;
            model.Permissions = string.Join(';', permissionKeys);
            model.UpdatedBy = $"{currentUser.FirstName} {currentUser.LastName}";
            await _roleRepository.Update(model);
        }

        [HttpPut]
        [Route("~/api/crosscompany/roles/{id}")]
        [PermissionAuthorize(Premissions.ManageRole, Premissions.ViewCompany)]
        public async Task UpdateCrosscompanyRole([FromRoute] string id, RoleEditModel model)
        {
            var currentUser = await GetCurrentUser();
            var role = await _roleRepository.Read(id);
            var permissionKeys = await PrunePermissions(model.CompanyId, model.PermissionKeys);
            role.Id = id;
            role.Description = model.Description;
            role.Active = model.Active;
            role.Name = model.Name;
            role.Permissions = string.Join(';', permissionKeys);
            role.CompanyId = model.CompanyId;
            role.UpdatedBy = $"{currentUser.FirstName} {currentUser.LastName}";
            await _roleRepository.Update(role);
        }

        [HttpDelete]
        [Route("~/api/crosscompany/roles/{id}")]
        [PermissionAuthorize(Premissions.ManageRole, Premissions.ViewCompany)]
        public async Task DeleteCrosscompanyRole([FromRoute] string id)
        {
            await _roleRepository.Delete(id);
        }

        private async Task<IList<string>> PrunePermissions(string companyId, IList<string> sourcePermissionKeys)
        {
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
                    dto.permissions = allPermissions.Where(p => t.PermissionKeys.Contains(p.Key));
                }

                return dto;
            });

            return result;
        }
    }
}
