using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNVGL.Authorization.UserManagement.Abstraction.Entity
{
    public class Company
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public string CreatedBy { get; set; }
        public string Permissions { get; set; }
        public IReadOnlyList<string> PermissionKeys => (Permissions ?? string.Empty).Split(';').ToList();
        public DateTime CreatedOnUtc { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
    }
}
