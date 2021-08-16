using System;
using System.Collections.Generic;
using System.Text;
using DNVGL.Authorization.Web.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace DNVGL.Authorization.UserManagement.ApiControllers
{
    public class UserManagementOptions
    {
        private UserManagementMode _mode = UserManagementMode.Company_CompanyRole_User;

        public UserManagementMode Mode
        {
            get { return _mode; }
            set
            {
                _mode = value;
            }
        }

        public Action<DbContextOptionsBuilder> DbContextOptionsBuilder { get; set; }
        public Action<ModelBuilder> ModelBuilder { get; set; }
        public PermissionOptions PermissionOptions { get; set; }
    }

    public enum UserManagementMode
    {
        Company_CompanyRole_User,
        Company_GlobalRole_User,
        Role_User,
    }
}
