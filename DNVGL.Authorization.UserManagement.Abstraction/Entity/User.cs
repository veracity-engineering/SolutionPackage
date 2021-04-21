using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNVGL.Authorization.UserManagement.Abstraction.Entity
{
    public class User
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string VeracityId { get; set; }
        public string Description { get; set; }
        public string RoleIds { get; set; }
        public IReadOnlyList<string> RoleIdList => RoleIds.SplitToList(';');
        public string CompanyId { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }

        public IReadOnlyList<Role> RoleList { get; set; }
        public Company Company { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
    }
}
