using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace DNVGL.Authorization.UserManagement.ApiControllers
{
    internal class ErrorCodeExceptionFilter : IExceptionFilter
    {
        private readonly ILogger _logger;
        public ErrorCodeExceptionFilter(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ErrorCodeExceptionFilter>();
        }

        public void OnException(ExceptionContext context)
        {
            var errorCode = _logger.LogExceptionAsError(context.Exception);
            context.Result = new ObjectResult(errorCode)
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
            };
        }
    }
}
