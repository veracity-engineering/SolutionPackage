using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace DNVGL.Authorization.Web
{
    public class PermissionValueAttribute : Attribute
    {
        public string Id { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Group { get; set; }

    }
}
