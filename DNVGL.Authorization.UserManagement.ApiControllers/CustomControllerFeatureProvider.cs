using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;

namespace DNVGL.Authorization.UserManagement.ApiControllers
{
    public class CustomControllerFeatureProvider : ControllerFeatureProvider
    {
        private readonly Type[] _hidenControllers;
        public CustomControllerFeatureProvider(Type[] hidenControllers)
        {
            _hidenControllers = hidenControllers;
        }


        protected override bool IsController(TypeInfo typeInfo)
        {
            var isController = base.IsController(typeInfo);

            if (isController)
            {
                isController = !_hidenControllers.Any(t => t.FullName == typeInfo.FullName);
            }

            return isController;
        }
    }
}
