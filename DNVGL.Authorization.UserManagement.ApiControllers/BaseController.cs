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
    public class UserManagementBaseController<TUser> : ControllerBase where TUser : User, new()
    {
        private readonly PermissionOptions _premissionOptions;
        private readonly IUser<TUser> _userRepository;

        protected UserManagementBaseController(IUser<TUser> userRepository, PermissionOptions premissionOptions)
        {
            _userRepository = userRepository;
            _premissionOptions = premissionOptions;
        }

        protected async Task<TUser> GetCurrentUser()
        {
            var varacityId = _premissionOptions.GetUserIdentity(HttpContext.User);
            return await _userRepository.ReadByIdentityId(varacityId);
        }

        protected async Task<bool> IsCompanyAccessible(string companyId)
        {
            var user = await GetCurrentUser();
            return user.CompanyIdList.Contains(companyId);
        }
    }
}
