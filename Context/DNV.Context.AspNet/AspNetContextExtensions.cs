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
		public static IApplicationBuilder UseAspNetContext<T>(this IApplicationBuilder builder, JsonSerializerSettings? jsonSerializerSettings = null) where T : class
		{
			return jsonSerializerSettings == null? 
				builder.UseMiddleware<AspNetContextMiddleware<T>>(): 
				builder.UseMiddleware<AspNetContextMiddleware<T>>(jsonSerializerSettings);
		}

		public static IServiceCollection AddAspNetContext<T>(this IServiceCollection services, Func<HttpContext, (bool succeeded, T context)> ctxCreator) where T : class
		{
			return services.AddScoped(_ => new AspNetContextAccessor<T>(ctxCreator))
				.AddScoped(sp => sp.GetRequiredService<AspNetContextAccessor<T>>() as IContextAccessor<T>);
		}
	}
}
