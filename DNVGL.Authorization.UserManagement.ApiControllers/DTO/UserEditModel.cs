using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DNVGL.Authorization.UserManagement.Abstraction.Entity;
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
        public IList<string> RoleIds { get; set; }
        public string CompanyId { get; set; }
        public bool Active { get; set; }
    }

    public class UserViewModel:User
    {
        private new string CompanyId { get; set; }

        private new string RoleIds { get; set; }

        private new IReadOnlyList<string> RoleIdList { get; set; }

        private new IReadOnlyList<Role> RoleList { get; set; }

        private new Company Company { get; set; }

        public IEnumerable<RoleViewDto> Roles { get; set; }

        public CompanyViewDto company { get; set; }

    }
}
