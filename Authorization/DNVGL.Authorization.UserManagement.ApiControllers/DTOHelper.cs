using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using DNVGL.Authorization.UserManagement.Abstraction.Entity;
using DNVGL.Authorization.UserManagement.ApiControllers.DTO;

namespace DNVGL.Authorization.UserManagement.ApiControllers
{
    internal static class DtoHelper
    {
        internal static R ToViewDto<R>(this object obj) where R : new()
        {
            if (obj == null)
                return default;

            Type type = typeof(R);
            Type declaringType = obj.GetType();
            PropertyInfo[] PropertyInfo = type.GetProperties();
            R result = new R();
            foreach (PropertyInfo item in PropertyInfo.Where(t => t.DeclaringType == declaringType))
            {
                item.SetValue(result, item.GetValue(obj));
            }

            return result;
        }
    }
}
