using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace DNVGL.Authorization.UserManagement.EFCore
{
    public class UserDatabaseOptions
    {
        public Action<DbContextOptionsBuilder> DbContextOptionsBuilder;
        public Action<ModelBuilder> ModelBuilder;
    }
}
