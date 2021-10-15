// Copyright (c) DNV. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
using System.Collections.Generic;
using System.Linq;

namespace DNVGL.Authorization.UserManagement.Abstraction
{
    /// <summary>
    /// string helper class.
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// Split a string and combine all sub strings as a list.
        /// </summary>
        /// <param name="source">The source stirng to split.</param>
        /// <param name="delimiter">The character delimit the string.</param>
        /// <returns></returns>
        public static List<string> SplitToList(this string source,char delimiter)
        {
            return source.Split(delimiter).ToList();
        }
    }
}
