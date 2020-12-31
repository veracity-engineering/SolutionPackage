﻿using System;
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
        public string CreatedBy { get; private set; }
        public DateTime CreatedOnUtc { get; private set; }
        public string UpdatedBy { get; private set; }
        public DateTime UpdatedOnUtc { get; private set; }
    }
}
