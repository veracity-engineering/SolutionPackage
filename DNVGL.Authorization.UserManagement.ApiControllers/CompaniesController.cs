﻿using System;
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
    [Produces("application/json")]
    [ApiController]
    [Route("api/companies")]
    [TypeFilter(typeof(ErrorCodeExceptionFilter))]
    [ApiExplorerSettings(GroupName = "UserManagement's Company APIs")]
    public class CompaniesController<TCompany, TUser> : UserManagementBaseController<TUser> where TCompany : Company, new() where TUser : User, new()
    {
        private readonly ICompany<TCompany> _companyRepository;
        private readonly IPermissionRepository _permissionRepository;

        public CompaniesController(ICompany<TCompany> companyRepository, IPermissionRepository permissionRepository, IUser<TUser> userRepository, PermissionOptions premissionOptions) : base(userRepository, premissionOptions)
        {
            _companyRepository = companyRepository;
            _permissionRepository = permissionRepository;
        }


        /// <summary>
        /// Get all companies.
        /// </summary>
        /// <remarks>
        /// Required Permission: ViewCompany
        /// </remarks>
        /// <returns>An array of company</returns>
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

        /// <summary>
        /// Get a company by companyId
        /// </summary>
        /// <remarks>
        /// Required Permission: ViewCompany
        /// </remarks>
        /// <param name="companyId">Company Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{companyId}")]
        [CompanyIdentityFieldNameFilter(companyIdInRoute: "companyId")]
        [AccessCrossCompanyPermissionFilter(Premissions.ViewCompany)]
        [AccessibleCompanyFilter]
        public async Task<CompanyViewDto> GetCompany([FromRoute] string companyId)
        {
            var company = await _companyRepository.Read(companyId);
            var allPermissions = await _permissionRepository.GetAll();
            var result = company.ToViewDto<CompanyViewDto>();
            result.permissions = allPermissions.Where(p => company.PermissionKeys.Contains(p.Key));

            return result;
        }


        [HttpGet]
        [Route("obsoletedapi/{id}")]
        [PermissionAuthorize(Premissions.ViewCompany)]
        [ObsoleteAttribute("It's an obsoleted end point. not suggest to use.", true)]
        public async Task<CompanyViewDto> GetCompanyAdmin([FromRoute] string id)
        {
            var company = await _companyRepository.Read(id);
            var allPermissions = await _permissionRepository.GetAll();
            var result = company.ToViewDto<CompanyViewDto>();
            result.permissions = allPermissions.Where(p => company.PermissionKeys.Contains(p.Key));

            return result;
        }

        /// <summary>
        /// Get a company from its dedicated domain url.
        /// </summary>
        /// <remarks>
        /// Required Permission: None
        /// </remarks>
        /// <param name="url">Company Domain URL</param>
        /// <returns></returns>
        [HttpGet]
        [Route("domain/{*url}")]
        public async Task<CompanyViewDto> GetCompanyByDomain([FromRoute] string url)
        {
            var currentUser = await GetCurrentUser();
            var decodedUrl = WebUtility.UrlDecode(url);
            var urlParts = decodedUrl.Replace("https://", "", StringComparison.InvariantCultureIgnoreCase).Replace("http://", "", StringComparison.InvariantCultureIgnoreCase).Split("/");

            TCompany company = null;
            if (urlParts.Length > 1)
            {
                company = await _companyRepository.ReadByDomain(urlParts[0] + "/" + urlParts[1]);
            }

            if (company == null)
            {
                company = await _companyRepository.ReadByDomain(urlParts[0]);
            }
            
            if (company == null && urlParts.Length > 1)
            {
                company = await _companyRepository.ReadByDomain(urlParts[1]);
            }

            if (company == null || (currentUser.CompanyList.All(t => t.Id != company.Id) && !currentUser.SuperAdmin))
            {
                return null;
            }

            var allPermissions = await _permissionRepository.GetAll();
            var result = company.ToViewDto<CompanyViewDto>();
            result.permissions = allPermissions.Where(p => company.PermissionKeys.Contains(p.Key));

            return result;
        }

        /// <summary>
        /// Create a company using custom model. Only if custom company model is used.
        /// </summary>
        /// <remarks>
        /// Required Permissions: ManageCompany
        /// 
        /// Sample request:
        ///
        ///     {
        ///        "name": "Item1",
        ///        "description": "",
        ///        "domainUrl": "",
        ///        "active":true,
        ///        "customProperty":"",
        ///        "permissionKeys":["ReadWeather","ManageWeather"]
        ///     }
        ///
        /// </remarks>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("custommodel")]
        [PermissionAuthorize(Premissions.ManageCompany)]
        [ApiExplorerSettings(GroupName = "UserManagement's Company APIs - Custom Model")]
        public async Task<string> CreateCompanyFromCustomModel([FromBody] TCompany model)
        {
            var currentUser = await GetCurrentUser();
            model.CreatedBy = $"{currentUser.FirstName} {currentUser.LastName}";
            model = await _companyRepository.Create(model);
            return model.Id;
        }

        /// <summary>
        /// Create a company.
        /// </summary>
        /// <remarks>
        /// Required Permissions: ManageCompany
        /// 
        /// Sample request:
        ///
        ///     {
        ///        "name": "Item1",
        ///        "description": "",
        ///        "domainUrl": "",
        ///        "active":true,
        ///        "permissionKeys":["ReadWeather","ManageWeather"]
        ///     }
        ///
        /// </remarks>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [PermissionAuthorize(Premissions.ManageCompany)]
        public async Task<string> CreateCompany([FromBody] CompanyEditModel model)
        {
            var currentUser = await GetCurrentUser();


            var company = new TCompany
            {
                Description = model.Description,
                Name = model.Name,
                Active = model.Active,
                Permissions = string.Join(';', model.PermissionKeys),
                DomainUrl = model.DomainUrl,
                CreatedBy = $"{currentUser.FirstName} {currentUser.LastName}"
            };
            company = await _companyRepository.Create(company);
            return company.Id;
        }

        /// <summary>
        /// Update a company using custom model. Only if custom company model is used.
        /// </summary>
        /// <remarks>
        /// Required Permissions: ManageCompany
        /// 
        /// Sample request:
        ///
        ///     {
        ///        "name": "Item1",
        ///        "description": "",
        ///        "domainUrl": "",
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
        [PermissionAuthorize(Premissions.ManageCompany)]
        [ApiExplorerSettings(GroupName = "UserManagement's Company APIs - Custom Model")]
        public async Task UpdateCompanyFromCustomModel([FromRoute] string id, TCompany model)
        {
            var company = await _companyRepository.Read(id);
            var currentUser = await GetCurrentUser();
            model.Id = company.Id;
            model.UpdatedBy = $"{currentUser.FirstName} {currentUser.LastName}";
            await _companyRepository.Update(model);
        }

        /// <summary>
        /// Update a company
        /// </summary>
        /// <remarks>
        /// Required Permissions: ManageCompany
        /// 
        /// Sample request:
        ///
        ///     {
        ///        "name": "Item1",
        ///        "description": "",
        ///        "domainUrl": "",
        ///        "active":true,
        ///        "permissionKeys":["ReadWeather","ManageWeather"]
        ///     }
        ///
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        [PermissionAuthorize(Premissions.ManageCompany)]
        public async Task UpdateCompany([FromRoute] string id, CompanyEditModel model)
        {
            var company = await _companyRepository.Read(id);
            var currentUser = await GetCurrentUser();
            company.Id = id;
            company.Active = model.Active;
            company.DomainUrl = model.DomainUrl;
            company.Description = model.Description;
            company.Name = model.Name;
            company.Permissions = string.Join(';', model.PermissionKeys);
            company.UpdatedBy = $"{currentUser.FirstName} {currentUser.LastName}";
            await _companyRepository.Update(company);
        }

        /// <summary>
        /// Delete a company. It is hard delete.
        /// </summary>
        /// <remarks>
        /// Required Permissions: ManageCompany
        /// </remarks>
        /// <param name="id">Company Id</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        [PermissionAuthorize(Premissions.ManageCompany)]
        public async Task DeleteCompany([FromRoute] string id)
        {
            await _companyRepository.Delete(id);
        }

    }
}
