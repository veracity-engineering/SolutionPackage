using System;
using System.Collections.Generic;
using System.Text;
using DNVGL.Authorization.Web.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace DNVGL.Authorization.UserManagement.ApiControllers
{
    public class UserManagementOptions
    {
        public UserManagementMode Mode = UserManagementMode.Company_CompanyRole_User;
        public Action<DbContextOptionsBuilder> DbContextOptionsBuilder;
        public Action<ModelBuilder> ModelBuilder;
        public PermissionOptions PermissionOptions;
    }

    public enum UserManagementMode
    {
        Company_CompanyRole_User,
        Company_GlobalRole_User,
        Role_User,
    }
}
