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
            PropertyInfo[] declaringPropertyInfo = obj.GetType().GetProperties();
            PropertyInfo[] PropertyInfo = type.GetProperties();
            R result = new R();
            foreach (PropertyInfo item in PropertyInfo)
            {

                var propertyInSource = declaringPropertyInfo.FirstOrDefault(p => p.Name == item.Name && p.PropertyType == item.PropertyType);
                if(propertyInSource != null)
				{
                    item.SetValue(result, propertyInSource.GetValue(obj));
                }
            }

            return result;
        }
    }
}
