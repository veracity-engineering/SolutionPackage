using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNVGL.Authorization.UserManagement.Abstraction
{
    public static class StringHelper
    {
        public static List<string> SplitToList(this string source,char delimiter)
        {
            return source.Split(delimiter).ToList();
        }
    }
}
