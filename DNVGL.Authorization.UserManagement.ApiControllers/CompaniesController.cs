using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DNVGL.Authorization.UserManagement.Abstraction;
using DNVGL.Authorization.UserManagement.Abstraction.Entity;
using DNVGL.Authorization.UserManagement.ApiControllers.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DNVGL.Authorization.UserManagement.ApiControllers
{
    [Authorize]
    [ApiController]
    [Route("api/companies")]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompany _companyRepository;

        public CompaniesController(ICompany companyRepository)
            => _companyRepository = companyRepository;


        [HttpGet]
        [Route("")]
        public async Task<IEnumerable<Company>> GetCompanys()
        {
            var result = await _companyRepository.All();

            return result;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<Company> GetCompany([FromRoute] string id)
        {
            var result = await _companyRepository.Read(id);
            return result;
        }

        [HttpPost]
        [Route("")]
        public async Task<string> CreateCompany([FromBody] CompanyEditModel model)
        {
            var company = new Company
            {
                Description = model.Description,
                Name = model.Name,
                Active = model.Active
            };
            company = await _companyRepository.Create(company);
            return company.Id;
        }

        [HttpPut]
        [Route("{id}")]
        public async Task UpdateCompany([FromRoute] string id, CompanyEditModel model)
        {
            var company = await _companyRepository.Read(id);
            company.Id = id;
            company.Active = model.Active;
            company.Description = model.Description;
            company.Name = model.Name;
            await _companyRepository.Update(company);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task DeleteCompany([FromRoute] string id)
        {
            await _companyRepository.Delete(id);
        }


    }
}
