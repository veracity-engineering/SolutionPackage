using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using DNV.Context.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace DNV.Context.AspNet
{
    public class AspNetContextMiddleware<T> where T: class
    {
        private readonly RequestDelegate _next;
        private readonly JsonSerializerOptions? _jsonSerializerOptions;

        public AspNetContextMiddleware(RequestDelegate next): this(next, null) { }

        public AspNetContextMiddleware(RequestDelegate next, JsonSerializerOptions? jsonSerializerOptions)
        {
            _next = next;
            _jsonSerializerOptions = jsonSerializerOptions;
        }

        public async Task Invoke(HttpContext context)
        {
	        var contextAccessor = context.RequestServices.GetRequiredService<AspNetContextAccessor<T>>();

            if (contextAccessor.Initialized)
            {
                await _next(context);
                return;
            }

            contextAccessor.Initialize(context, _jsonSerializerOptions);

            await _next(context);
        }
    }
}