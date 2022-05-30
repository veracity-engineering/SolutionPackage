using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DNVGL.Authorization.Web.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DNVGL.Authorization.Web
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "UserManagement's Permission APIs")]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionRepository _permissionRepository;
        public PermissionController(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }


        [HttpGet]
        public async Task<IEnumerable<PermissionEntity>> Get()
        {
            return (await _permissionRepository.GetAll()).Where(t => t.Key != "ViewCompany" && t.Key != "ManageCompany");
        }
    }
}
