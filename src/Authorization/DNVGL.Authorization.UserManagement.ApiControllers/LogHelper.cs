using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace DNVGL.Authorization.UserManagement.ApiControllers
{
    internal static class LogHelper
    {
        internal static object LogExceptionAsError(this ILogger logger, Exception ex)
        {
            var errorCode = Guid.NewGuid();

            using (logger.BeginScope(new Dictionary<string, object> { { "errorCode", errorCode } }))
            {
                logger.LogError(ex, ex.Message);
            }

            return errorCode;
        }
    }
}
