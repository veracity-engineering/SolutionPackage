using System;
using System.Collections.Generic;
using System.Text;

namespace DNVGL.Authorization.UserManagement.ApiControllers.DTO
{
    public class CompanyEditModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
    }
}
