using System;
using System.Collections.Generic;
using System.Text;

namespace DNVGL.Authorization.UserManagement.Abstraction
{
    public class UserManagementSettings
    {
        public UserManagementMode Mode { get; set; }
    }

    public enum UserManagementMode
    {
        Company_CompanyRole_User,
        Company_GlobalRole_User,
        Role_User,
    }
}
