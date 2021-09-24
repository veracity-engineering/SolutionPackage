using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DNVGL.Authorization.Web.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace DNVGL.Authorization.Web
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class CompanyIdentityFieldNameFilterAttribute : TypeFilterAttribute
    {
        private readonly string _companyIdInRoute;
        private readonly string _companyIdInQuery;
        private readonly string _companyIdInActionArguments;

        public CompanyIdentityFieldNameFilterAttribute(string companyIdInRoute = "", string companyIdInQuery = "", string companyIdInActionArguments = "")
                : base(typeof(CompanyIdentityFieldNameFilterImpl))
        {
            Arguments = new object[] { companyIdInRoute, companyIdInQuery, companyIdInActionArguments };
            Order = 1;

            _companyIdInRoute = companyIdInRoute;
            _companyIdInQuery = companyIdInQuery;
            _companyIdInActionArguments = companyIdInActionArguments;

            if (string.IsNullOrEmpty(_companyIdInRoute) && string.IsNullOrEmpty(_companyIdInQuery) && string.IsNullOrEmpty(_companyIdInActionArguments))
            {
                _companyIdInRoute = "companyId";
            }
        }

        public void GetCompanyId(HttpContext context)
        {
            var companyId = context.GetRouteData().Values[_companyIdInRoute] as string ?? context.Request.Query[_companyIdInQuery];
            if (!string.IsNullOrEmpty(companyId))
            {
                context.Request.Headers.Remove("AUTHORIZATION.COMPANYID");
                context.Request.Headers.Add("AUTHORIZATION.COMPANYID", companyId);
            }
        }

        private class CompanyIdentityFieldNameFilterImpl : IAsyncActionFilter
        {
            private readonly PermissionOptions _premissionOptions;
            private readonly string _companyIdInRoute;
            private readonly string _companyIdInQuery;
            private readonly string _companyIdInActionArguments;
            public CompanyIdentityFieldNameFilterImpl(string companyIdInRoute, string companyIdInQuery, string companyIdInActionArguments, PermissionOptions premissionOptions)
            {
                _companyIdInRoute = companyIdInRoute;
                _companyIdInQuery = companyIdInQuery;
                _companyIdInActionArguments = companyIdInActionArguments;

                if (string.IsNullOrEmpty(_companyIdInRoute) && string.IsNullOrEmpty(_companyIdInQuery) && string.IsNullOrEmpty(_companyIdInActionArguments))
                {
                    _companyIdInRoute = "companyId";
                }
                _premissionOptions = premissionOptions;
            }

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {

                var companyId = GetCompanyId(context);

                if (!string.IsNullOrEmpty(companyId))
                {
                    context.HttpContext.Request.Headers.Remove("AUTHORIZATION.COMPANYID");
                    context.HttpContext.Request.Headers.Add("AUTHORIZATION.COMPANYID", companyId);
                    
                }

                await next();
            }



            private string GetCompanyId(ActionExecutingContext context)
            {
                string companyId = context.HttpContext.GetRouteData().Values[_companyIdInRoute] as string ?? context.HttpContext.Request.Query[_companyIdInQuery];

                if (string.IsNullOrEmpty(companyId) && context.ActionArguments.TryGetValue(_companyIdInActionArguments, out object value) && value is string companyIdStr)
                {
                    companyId = companyIdStr;
                }
                else if (string.IsNullOrEmpty(companyId) && _premissionOptions.GetCompanyIdentity!=null)
                {
                    companyId = _premissionOptions.GetCompanyIdentity(context.HttpContext);
                }

                return companyId;
            }
        }
    }
}
