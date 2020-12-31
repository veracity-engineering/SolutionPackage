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
    [Route("api/roles")]
    public class RolesController : ControllerBase
    {
        private readonly IRole _roleRepository;

        public RolesController(IRole roleRepository)
            => _roleRepository = roleRepository;
    }
}
