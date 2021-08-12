using System;
using System.Collections.Generic;
using System.Text;
using DNVGL.Authorization.Web.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace DNVGL.Authorization.UserManagement.ApiControllers
{
    public class UserManagementOptions
    {
        public Action<DbContextOptionsBuilder> DbContextOptionsBuilder;
        public Action<ModelBuilder> ModelBuilder;
        public PermissionOptions PermissionOptions;
    }
}
