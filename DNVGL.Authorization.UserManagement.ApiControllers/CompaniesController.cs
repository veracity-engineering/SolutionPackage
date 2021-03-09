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
    [Route("api/companies")]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompany _companyRepository;
        private readonly IPermissionRepository _permissionRepository;

        public CompaniesController(ICompany companyRepository, IPermissionRepository permissionRepository)
        {
            _companyRepository = companyRepository;
            _permissionRepository = permissionRepository;
        }



        [HttpGet]
        [Route("")]
        [PermissionAuthorize(Premissions.ViewCompany)]
        public async Task<IEnumerable<CompanyViewDto>> GetCompanys()
        {
            var companys = await _companyRepository.All();
            var allPermissions = await _permissionRepository.GetAll();


            var result = companys.Select(t =>
            {
                var dto = t.ToViewDto<CompanyViewDto>();

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
        [PermissionAuthorize(Premissions.ViewCompany)]
        public async Task<Company> GetCompany([FromRoute] string id)
        {
            var company = await _companyRepository.Read(id);
            var allPermissions = await _permissionRepository.GetAll();
            var result = company.ToViewDto<CompanyViewDto>();
            result.permissions = allPermissions.Where(p => company.PermissionKeys.Contains(p.Key));

            return result;
        }

        [HttpPost]
        [Route("")]
        [PermissionAuthorize(Premissions.ManageCompany)]
        public async Task<string> CreateCompany([FromBody] CompanyEditModel model)
        {
            var company = new Company
            {
                Description = model.Description,
                Name = model.Name,
                Active = model.Active,
                Permissions = string.Join(';', model.PermissionKeys)
            };
            company = await _companyRepository.Create(company);
            return company.Id;
        }

        [HttpPut]
        [Route("{id}")]
        [PermissionAuthorize(Premissions.ManageCompany)]
        public async Task UpdateCompany([FromRoute] string id, CompanyEditModel model)
        {
            var company = await _companyRepository.Read(id);
            company.Id = id;
            company.Active = model.Active;
            company.Description = model.Description;
            company.Name = model.Name;
            company.Permissions = string.Join(';', model.PermissionKeys);
            await _companyRepository.Update(company);
        }

        [HttpDelete]
        [Route("{id}")]
        [PermissionAuthorize(Premissions.ManageCompany)]
        public async Task DeleteCompany([FromRoute] string id)
        {
            await _companyRepository.Delete(id);
        }


    }
}
