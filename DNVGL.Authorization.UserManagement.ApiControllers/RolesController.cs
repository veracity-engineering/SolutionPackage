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
    [Route("api/roles")]
    public class RolesController : ControllerBase
    {
        private readonly IRole _roleRepository;
        private readonly ICompany _companyRepository;
        private readonly IPermissionRepository _permissionRepository;

        public RolesController(IRole roleRepository, ICompany companyRepository, IPermissionRepository permissionRepository)
        {
            _roleRepository = roleRepository;
            _companyRepository = companyRepository;
            _permissionRepository = permissionRepository;
        }


        [HttpGet]
        [Route("")]
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
        [Route("company/{id}")]
        [PermissionAuthorize(Premissions.ViewRole)]
        public async Task<IEnumerable<RoleViewDto>> GetCompanyRoles([FromRoute] string id)
        {
            var roles = await _roleRepository.GetRolesOfCompany(id);
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
        [Route("{id}")]
        [PermissionAuthorize(Premissions.ViewRole)]
        public async Task<Role> GetRole([FromRoute] string id)
        {
            var role = await _roleRepository.Read(id);
            var allPermissions = await _permissionRepository.GetAll();
            var result = role.ToViewDto<RoleViewDto>();
            result.permissions = allPermissions.Where(p => role.PermissionKeys.Contains(p.Key));

            return result;
        }

        [HttpPost]
        [Route("")]
        [PermissionAuthorize(Premissions.ManageRole)]
        public async Task<string> CreateRole([FromBody] RoleEditModel model)
        {
            var permissionKeys = await PrunePermissions(model.CompanyId, model.PermissionKeys);

            var role = new Role
            {
                Description = model.Description,
                Name = model.Name,
                Active = model.Active,
                CompanyId = model.CompanyId,
                Permissions = string.Join(';', permissionKeys)
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
            var permissionKeys = await PrunePermissions(model.CompanyId, model.PermissionKeys);
            role.Id = id;
            role.Active = model.Active;
            role.Description = model.Description;
            role.Name = model.Name;
            role.CompanyId = model.CompanyId;
            role.Permissions = string.Join(';', permissionKeys);
            await _roleRepository.Update(role);
        }

        [HttpDelete]
        [Route("{id}")]
        [PermissionAuthorize(Premissions.ManageRole)]
        public async Task DeleteRole([FromRoute] string id)
        {
            await _roleRepository.Delete(id);
        }

        private async Task<IList<string>> PrunePermissions(string companyId, IList<string> sourcePermissionKeys)
        {
            var company = await _companyRepository.Read(companyId);
            return sourcePermissionKeys.Where(t => company.PermissionKeys.Any(f => f == t)).ToList();
        }
    }
}
