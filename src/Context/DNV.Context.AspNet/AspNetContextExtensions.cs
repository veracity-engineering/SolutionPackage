using System;
using System.Text.Json;
using DNV.Context.Abstractions;
using DNVGL.Common.Core.JsonOptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace DNV.Context.AspNet
{
    public static class AspNetContextExtensions
    {
        public static IApplicationBuilder UseAspNetContext<T>(this IApplicationBuilder builder) where T : class
        {
            return builder.UseMiddleware<AspNetContextMiddleware<T>>(builder.ApplicationServices.GetService<IOptions<JsonSerializerOptions>>());
        }

        public static IServiceCollection AddAspNetContext<T>(this IServiceCollection services, Func<HttpContext, (bool succeeded, T? context)> ctxCreator, Action<JsonSerializerOptions>? jsonOptionsSetup = null) where T : class
        {
            if (jsonOptionsSetup == null)
                services.AddWebDefaultJsonOptions();
            else
                services.AddOptions().Configure(jsonOptionsSetup);

            services.TryAddSingleton(_ => new AspNetContextAccessor<T>(ctxCreator));

            return services.AddSingleton(sp => sp.GetRequiredService<AspNetContextAccessor<T>>() as IContextAccessor<T>)
                .AddSingleton(sp => sp.GetRequiredService<AspNetContextAccessor<T>>() as IContextCreator<T>);
        }
    }
}
