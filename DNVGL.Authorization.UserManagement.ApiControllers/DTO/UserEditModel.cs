using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace DNVGL.Authorization.UserManagement.ApiControllers.DTO
{
    public class UserEditModel
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string VeracityId { get; set; }
        public string Description { get; set; }
        [JsonIgnore]
        public string RoleIds { get; set; }

        public IList<string> RoleIdList => RoleIds.Split(';').ToList();
        public string CompanyId { get; set; }
        public bool Active { get; set; }
    }
}
