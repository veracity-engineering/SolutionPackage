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
    public class RolesController : UserManagementBaseController
    {
        private readonly IRole _roleRepository;
        private readonly ICompany _companyRepository;
        private readonly IPermissionRepository _permissionRepository;

        public RolesController(IUser userRepository, IRole roleRepository, ICompany companyRepository, IPermissionRepository permissionRepository, PermissionOptions premissionOptions) : base(userRepository, premissionOptions)
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
        public async Task<Role> GetRole([FromRoute] string companyId,[FromRoute] string id)
        {
            var roles = await GetCompanyRoles(companyId);

            if (roles.Any(t => t.Id == id))
            {
                return await FetchRole(id);
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        [Route("")]
        [PermissionAuthorize(Premissions.ManageRole)]
        public async Task<string> CreateRole([FromRoute] string companyId,[FromBody] RoleEditModel model)
        {
            var user = await GetCurrentUser();
            var permissionKeys = await PrunePermissions(companyId, model.PermissionKeys);

            var role = new Role
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
        public async Task<Role> GetCrosscompanyRole([FromRoute] string id)
        {
            return await FetchRole(id);
        }


        [HttpPost]
        [Route("~/api/crosscompany/roles")]
        [PermissionAuthorize(Premissions.ManageRole, Premissions.ViewCompany)]
        public async Task<string> CreateCrosscompanyRole([FromBody] RoleEditModel model)
        {
            var permissionKeys = await PrunePermissions(model.CompanyId, model.PermissionKeys);
            var currentUser = await GetCurrentUser();

            var role = new Role
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

        private async Task<Role> FetchRole(string id)
        {
            var role = await _roleRepository.Read(id);
            var allPermissions = await _permissionRepository.GetAll();
            var result = role.ToViewDto<RoleViewDto>();
            result.permissions = allPermissions.Where(p => role.PermissionKeys.Contains(p.Key));

            return result;
        }
    }
}
