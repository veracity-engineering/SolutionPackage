using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DNVGL.Authorization.UserManagement.Abstraction;
using DNVGL.Authorization.UserManagement.Abstraction.Entity;
using DNVGL.Authorization.Web.Abstraction;
using Microsoft.AspNetCore.Mvc;

namespace DNVGL.Authorization.UserManagement.ApiControllers
{
    public class UserManagementBaseController : ControllerBase
    {
        private readonly PermissionOptions _premissionOptions;
        private readonly IUser _userRepository;

        protected UserManagementBaseController(IUser userRepository, PermissionOptions premissionOptions)
        {
            _userRepository = userRepository;
            _premissionOptions = premissionOptions;
        }

        protected async Task<User> GetCurrentUser()
        {
            var varacityId = _premissionOptions.GetUserIdentity(HttpContext);
            return await _userRepository.ReadByIdentityId(varacityId);
        }

        protected async Task<bool> IsCompanyAccessible(string companyId)
        {
            var user = await GetCurrentUser();
            return user.CompanyIdList.Contains(companyId);
        }
    }
}
