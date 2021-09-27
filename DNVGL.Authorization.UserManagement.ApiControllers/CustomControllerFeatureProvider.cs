using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;

namespace DNVGL.Authorization.UserManagement.ApiControllers
{
    internal class CustomControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        private readonly Type[] _visibleControllers;
        public CustomControllerFeatureProvider(Type[] visibleControllers)
        {
            _visibleControllers = visibleControllers;
        }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            _visibleControllers.ToList().ForEach(t => { feature.Controllers.Add(t.GetTypeInfo()); });
        }
    }
}
