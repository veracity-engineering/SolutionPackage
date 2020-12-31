using System;
using System.Collections.Generic;
using System.Text;
using DNVGL.Authorization.UserManagement.Abstraction;
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
    }
}
