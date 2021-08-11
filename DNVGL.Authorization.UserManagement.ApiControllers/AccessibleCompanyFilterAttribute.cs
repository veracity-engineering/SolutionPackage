using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DNVGL.Authorization.UserManagement.Abstraction;
using DNVGL.Authorization.Web.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DNVGL.Authorization.UserManagement.ApiControllers
{
    public class AccessibleCompanyFilterAttribute : TypeFilterAttribute
    {
        public AccessibleCompanyFilterAttribute()
                        : base(typeof(AccessibleCompanyFilterImpl))
        {
        }

        private class AccessibleCompanyFilterImpl : IAsyncActionFilter
        {
            private readonly PermissionOptions _premissionOptions;
            private readonly IUser _userRepository;
            public AccessibleCompanyFilterImpl(IUser userRepository, PermissionOptions premissionOptions)
            {
                _userRepository = userRepository;
                _premissionOptions = premissionOptions;
            }

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                if (context.ActionArguments.TryGetValue("companyId", out object value) && value is string companyId)
                {
                    var varacityId = _premissionOptions.GetUserIdentity(context.HttpContext);
                    var user = await _userRepository.ReadByIdentityId(varacityId);
                    if (user.CompanyIdList.Contains(companyId))
                    {
                        await next();
                    }

                    context.Result = new ObjectResult(companyId + " is not accessible")
                    {
                        StatusCode = (int)HttpStatusCode.Forbidden,
                    };
                }
                else
                {
                    context.Result = new ObjectResult("companyId is required")
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest,
                    };
                }


            }
        }
    }
}
