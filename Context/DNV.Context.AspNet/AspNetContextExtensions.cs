using System;
using DNV.Context.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace DNV.Context.AspNet
{
	public static class AspNetContextExtensions
	{
		public static IApplicationBuilder AddAspNetContext<T>(this IApplicationBuilder builder, JsonSerializerSettings? jsonSerializerSettings) where T : class
		{
			return builder.UseMiddleware<AspNetContextMiddleware<T>>(jsonSerializerSettings);
		}

		public static IServiceCollection UseAspNetContext<T>(this IServiceCollection services, Func<HttpContext, T> ctxCreator) where T : class
		{
			return services.AddScoped(_ => new AspNetContextAccessor<T>(ctxCreator))
				.AddScoped(sp => sp.GetRequiredService<AspNetContextAccessor<T>>() as IContextAccessor<T>);
		}
	}
}
