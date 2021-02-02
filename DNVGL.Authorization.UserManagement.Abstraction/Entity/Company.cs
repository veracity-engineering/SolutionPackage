using System;
using System.Collections.Generic;
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
        public DateTime CreatedOnUtc { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
    }
}
