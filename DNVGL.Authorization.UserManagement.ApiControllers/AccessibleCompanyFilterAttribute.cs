using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DNVGL.Authorization.UserManagement.Abstraction;
using DNVGL.Authorization.UserManagement.Abstraction.Entity;
using DNVGL.Authorization.Web.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace DNVGL.Authorization.UserManagement.ApiControllers
{
    public class AccessibleCompanyFilterAttribute : TypeFilterAttribute
    {
        public AccessibleCompanyFilterAttribute()
                        : base(typeof(AccessibleCompanyFilterImpl<User>))
        {
          
        }

        private class AccessibleCompanyFilterImpl<TUser> : IAsyncActionFilter where TUser:User,new()
        {
            private readonly PermissionOptions _premissionOptions;
            private readonly IUser<TUser> _userRepository;
            public AccessibleCompanyFilterImpl(IUser<TUser> userRepository, PermissionOptions premissionOptions)
            {
                _userRepository = userRepository;
                _premissionOptions = premissionOptions;
            }

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {

                var companyId = GetCompanyId(context);

                if (!string.IsNullOrEmpty(companyId))
                {
                    var varacityId = _premissionOptions.GetUserIdentity(context.HttpContext.User);
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
                    context.Result = new ObjectResult("Company Id is required")
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest,
                    };
                }


            }

            private string GetCompanyId(ActionExecutingContext context)
            {
                string companyId = context.HttpContext.Request.Headers["AUTHORIZATION.COMPANYID"];

                return companyId;
            }
        }
    }
}
