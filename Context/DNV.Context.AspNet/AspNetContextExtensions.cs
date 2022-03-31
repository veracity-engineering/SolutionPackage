using System;
using System.Text.Json;
using DNV.Context.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace DNV.Context.AspNet
{
	public static class AspNetContextExtensions
	{
		public static IApplicationBuilder UseAspNetContext<T>(this IApplicationBuilder builder, JsonSerializerOptions? jsonSerializerOptions = null) where T : class
		{
			return jsonSerializerOptions == null? 
				builder.UseMiddleware<AspNetContextMiddleware<T>>(): 
				builder.UseMiddleware<AspNetContextMiddleware<T>>(jsonSerializerOptions);
		}

		public static IServiceCollection AddAspNetContext<T>(this IServiceCollection services, Func<HttpContext, (bool succeeded, T? context)> ctxCreator) where T : class
		{
			return services.AddSingleton(_ => new AspNetContextAccessor<T>(ctxCreator))
				.AddSingleton(sp => sp.GetRequiredService<AspNetContextAccessor<T>>() as IContextAccessor<T>);
		}
	}
}
