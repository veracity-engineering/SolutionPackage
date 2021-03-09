using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using DNVGL.Authorization.UserManagement.Abstraction.Entity;
using DNVGL.Authorization.Web;

namespace DNVGL.Authorization.UserManagement.ApiControllers.DTO
{
    public class CompanyEditModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public IEnumerable<string> PermissionKeys { get; set; }
    }

    public class CompanyViewDto : Company
    {
        private new string Permissions { get; set; }

        private new IReadOnlyList<string> PermissionKeys { get; set; }

        public IEnumerable<PermissionEntity> Permission { get; set; }
    }
}
