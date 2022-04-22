using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using DNV.Context.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DNV.Context.AspNet
{
    public class AspNetContextMiddleware<T> where T: class
    {
        private readonly RequestDelegate _next;
        private readonly JsonSerializerOptions? _jsonSerializerOptions;

        public AspNetContextMiddleware(RequestDelegate next, IOptions<JsonSerializerOptions>? jsonSerializerOptions)
        {
            _next = next;
            _jsonSerializerOptions = jsonSerializerOptions?.Value;
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