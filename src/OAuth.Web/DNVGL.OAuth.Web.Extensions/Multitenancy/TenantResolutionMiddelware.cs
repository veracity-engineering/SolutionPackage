using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace DNV.OAuth.Web.Extensions.Multitenancy
{
	internal class TenantResolutionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly OpenIdConnectOptions _oidcOptions;
        private readonly Func<PathString, bool>? _shouldSkip;

        public TenantResolutionMiddleware(RequestDelegate next, IOptionsMonitor<OpenIdConnectOptions> oidcOptions, Func<PathString, bool>? shouldSkip = null)
        {
            _next = next;
            _oidcOptions = oidcOptions.CurrentValue;
            _shouldSkip = shouldSkip;
        }

        public async Task Invoke(HttpContext context)
        {
	        if (!(_shouldSkip?.Invoke(context.Request.Path)??false))
	        {
		        var (tenantAlias, remainingPath) = await GetTenantAndPathFrom(context);

		        if (!string.IsNullOrEmpty(tenantAlias))
		        {
			        context.Request.PathBase = $"/{tenantAlias}";
			        context.Request.Path = remainingPath;
		        }
	        }

	        await _next(context);
        }

        private async Task<(string? tenantAlias, string? remainingPath)> GetTenantAndPathFrom(HttpContext context)
        {
            // example: https://domain/tenant1 -> gives tenant1
            var currentUri = new Uri(context.Request.GetDisplayUrl());
            var tenantAlias = currentUri
                .Segments
                .FirstOrDefault(x => x != "/")
                ?.TrimEnd('/');

            var currentPath = context.Request.Path;

            if (string.Equals(tenantAlias, _oidcOptions.CallbackPath.Value.Trim('/'), StringComparison.OrdinalIgnoreCase))
            {
	            var properties = await GetPropertiesFromState(context);

                if (properties != null && !string.IsNullOrEmpty(properties.RedirectUri) && !(_shouldSkip?.Invoke(properties.RedirectUri)??false))
                {
	                tenantAlias = new Uri(currentUri, properties.RedirectUri)
                        .Segments
                        .FirstOrDefault(x => x != "/")
                        ?.TrimEnd('/');

                    return (tenantAlias, _oidcOptions.CallbackPath);
                }

                tenantAlias = null;
            } 
            else if (string.Equals(tenantAlias, _oidcOptions.SignedOutCallbackPath.Value.Trim('/'),
	                       StringComparison.OrdinalIgnoreCase))
            {
	            tenantAlias = null;
            }

            if (!string.IsNullOrWhiteSpace(tenantAlias)
                && currentPath.StartsWithSegments($"/{tenantAlias}", out var remainingPath))
            {
                return (tenantAlias, remainingPath);
            }

            return (null, null);
        }

        private static async Task<AuthenticationProperties?> GetPropertiesFromState(HttpContext context)
        {
	        if (context.RequestServices.GetRequiredService<IAuthenticationService>() is not AuthenticationService authService 
	            || await authService.Handlers.GetHandlerAsync(context, OpenIdConnectDefaults.AuthenticationScheme) is not OpenIdConnectHandler handler) 
		        return null;

	        var message = new OpenIdConnectMessage((await context.Request.ReadFormAsync()).Select(pair => new KeyValuePair<string, string[]>(pair.Key, (string[])pair.Value)));

	        if (string.IsNullOrEmpty(message.State))
		        return default;

	        var properties = handler.Options.StateDataFormat.Unprotect(message.State);

	        return properties;

        }
    }
}
