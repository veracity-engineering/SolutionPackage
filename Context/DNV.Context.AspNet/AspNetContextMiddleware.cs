using System.IO;
using System.Threading.Tasks;
using DNV.Context.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace DNV.Context.AspNet
{
    public class AspNetContextMiddleware<T> where T: class
    {
        private readonly RequestDelegate _next;
        private readonly JsonSerializerSettings? _jsonSerializerSettings;

        public AspNetContextMiddleware(RequestDelegate next): this(next, null) { }

        public AspNetContextMiddleware(RequestDelegate next, JsonSerializerSettings? jsonSerializerSettings)
        {
            _next = next;
            _jsonSerializerSettings = jsonSerializerSettings;
        }

        public async Task Invoke(HttpContext context)
        {
	        var contextAccessor = context.RequestServices.GetRequiredService<AspNetContextAccessor<T>>();

            if (contextAccessor.Initialized)
            {
                await _next(context);
                return;
            }

            contextAccessor.Initialize(context, _jsonSerializerSettings);

            await _next(context);
        }
    }
}