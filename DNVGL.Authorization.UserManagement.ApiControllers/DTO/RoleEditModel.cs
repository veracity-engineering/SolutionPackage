using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
}
