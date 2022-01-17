// Copyright (c) DNV. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
using System;
using System.Threading.Tasks;
using DNVGL.Authorization.Web.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace DNVGL.Authorization.Web
{
    /// <summary>
    /// Provides a api controller filter to get a company id.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class CompanyIdentityFieldNameFilterAttribute : TypeFilterAttribute
    {
        private readonly string _companyIdInRoute;
        private readonly string _companyIdInQuery;
        private readonly string _companyIdInActionArguments;

        /// <summary>
        /// Constructs a new instance of <see cref="CompanyIdentityFieldNameFilterAttribute"/>.
        /// </summary>
        /// <param name="companyIdInRoute">compnay id specification in route.</param>
        /// <param name="companyIdInQuery">compnay id specification in query.</param>
        /// <param name="companyIdInActionArguments">compnay id specification in action arguments.</param>
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
                _companyIdInRoute = Constants.COMPANYID;
            }
        }

        /// <summary>
        /// Get company from httpcontext.
        /// </summary>
        /// <param name="context"><see cref="HttpContext"/></param>
        public void GetCompanyId(HttpContext context)
        {
            var companyId = context.GetRouteData().Values[_companyIdInRoute] as string ?? context.Request.Query[_companyIdInQuery];
            if (!string.IsNullOrEmpty(companyId))
            {
                context.Request.Headers.Remove(Constants.AUTHORIZATION_COMPANYID);
                context.Request.Headers.Add(Constants.AUTHORIZATION_COMPANYID, companyId);
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
                    _companyIdInRoute = Constants.COMPANYID;
                }
                _premissionOptions = premissionOptions;
            }

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {

                var companyId = GetCompanyId(context);

                if (!string.IsNullOrEmpty(companyId))
                {
                    context.HttpContext.Request.Headers.Remove(Constants.AUTHORIZATION_COMPANYID);
                    context.HttpContext.Request.Headers.Add(Constants.AUTHORIZATION_COMPANYID, companyId);
                    
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
