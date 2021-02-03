using System;
using System.Collections.Generic;
using System.Text;

namespace DNVGL.Authorization.UserManagement.ApiControllers.DTO
{
    public class UserEditModel
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string VeracityId { get; set; }
        public string Description { get; set; }
        public string RoleId { get; set; }
        public string CompanyId { get; set; }
        public bool Active { get; set; }
    }
}
