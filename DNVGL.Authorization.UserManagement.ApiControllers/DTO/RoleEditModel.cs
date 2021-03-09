using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DNVGL.Authorization.UserManagement.Abstraction.Entity;
using DNVGL.Authorization.Web;
using Newtonsoft.Json;

namespace DNVGL.Authorization.UserManagement.ApiControllers.DTO
{
    public class RoleEditModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }

        [JsonIgnore]
        public string Permissions { get; set; }

        public IList<string> PermissionKeys => Permissions.Split(';').ToList();
    }


    public class RoleViewDto : Role
    {
        private new string Permissions { get; set; }

        private new IReadOnlyList<string> PermissionKeys { get; set; }

        public IEnumerable<PermissionEntity> Permission { get; set; }
    }
}
