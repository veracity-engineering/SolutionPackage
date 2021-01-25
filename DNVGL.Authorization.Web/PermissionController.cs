using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DNVGL.Authorization.Web.Abstraction;
using Microsoft.AspNetCore.Mvc;

namespace DNVGL.Authorization.Web
{
    [ApiController]
    [Route("[controller]")]
    public class PermissionController : ControllerBase
    {
        private IPermissionRepository _permissionRepository;
        public PermissionController(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }


        [HttpGet]
        public async Task<IEnumerable<PermissionEntity>> Get()
        {
            return await _permissionRepository.GetAll();
        }
    }
}
