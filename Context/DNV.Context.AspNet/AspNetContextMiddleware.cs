using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace DNV.Context.AspNet
{
    public class AspNetContextMiddleware<T> where T: class
    {
        private readonly RequestDelegate _next;
        private readonly JsonSerializerSettings? _jsonSerializerSettings;

        public AspNetContextMiddleware(RequestDelegate next, JsonSerializerSettings? jsonSerializerSettings = null)
        {
            _next = next;
            _jsonSerializerSettings = jsonSerializerSettings;
        }

        public async Task Invoke(HttpContext context)
        {
	        var contextAccessor = context.RequestServices.GetRequiredService<AspNetContextAccessor<T>>();

            if (contextAccessor.Current != null)
            {
                await _next(context);
                return;
            }

	        var aspNetContext = context.RequestServices.GetRequiredService<AspNetContext<T>>();

            if (context.Request.Headers.TryGetValue(aspNetContext.Key, out var ctxJsonStr))
            {
	            var serializer = JsonSerializer.CreateDefault(_jsonSerializerSettings);

	            using var sr = new StringReader(ctxJsonStr);
	            using var jr = new JsonTextReader(sr);
	            var ctx = serializer.Deserialize<T>(jr);

                if (ctx != null)
					contextAccessor.CreateContext(ctx);
            }
            else
            {
	            contextAccessor.CreateContext(context);
            }

            await _next(context);
        }
    }
}