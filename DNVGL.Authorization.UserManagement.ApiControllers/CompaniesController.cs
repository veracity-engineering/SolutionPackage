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
using Microsoft.Extensions.Logging;
using static DNVGL.Authorization.Web.PermissionMatrix;

namespace DNVGL.Authorization.UserManagement.ApiControllers
{
    [Authorize]
    [ApiController]
    [Route("api/companies")]
    [TypeFilter(typeof(ErrorCodeExceptionFilter))]
    public class CompaniesController : UserManagementBaseController
    {
        private readonly ICompany _companyRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IUser _userRepository;
        private readonly PermissionOptions _premissionOptions;

        public CompaniesController(ICompany companyRepository, IPermissionRepository permissionRepository, IUser userRepository, PermissionOptions premissionOptions) : base(userRepository, premissionOptions)
        {
            _companyRepository = companyRepository;
            _userRepository = userRepository;
            _premissionOptions = premissionOptions;
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
            var currentUser = await GetCurrentUser();

            var company = new Company
            {
                Description = model.Description,
                Name = model.Name,
                Active = model.Active,
                Permissions = string.Join(';', model.PermissionKeys),
                CreatedBy = $"{currentUser.FirstName} {currentUser.LastName}"
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
            var currentUser = await GetCurrentUser();
            company.Id = id;
            company.Active = model.Active;
            company.Description = model.Description;
            company.Name = model.Name;
            company.Permissions = string.Join(';', model.PermissionKeys);
            company.UpdatedBy = $"{currentUser.FirstName} {currentUser.LastName}";
            await _companyRepository.Update(company);
        }

        [HttpDelete]
        [Route("{id}")]
        [PermissionAuthorize(Premissions.ManageCompany)]
        public async Task DeleteCompany([FromRoute] string id)
        {
            await _companyRepository.Delete(id);
        }

        //private async Task<User> GetCurrentUser()
        //{
        //    var varacityId = _premissionOptions.GetUserIdentity(HttpContext);
        //    return await _userRepository.ReadByIdentityId(varacityId);
        //}

    }
}
