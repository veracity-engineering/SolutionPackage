using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using DNV.Context.Abstractions;
using DNV.Context.AspNet;
using Microsoft.Extensions.Options;

namespace DNV.Context.HttpClient
{
    internal class HttpClientContextHandler<T> : DelegatingHandler where T : class
    {
	    public static readonly string ClientName = typeof(T).FullName;

        private readonly IContextAccessor<T> _contextAccessor;
        private readonly JsonSerializerOptions? _jsonSerializerOptions;

        public HttpClientContextHandler(IContextAccessor<T> contextAccessor, IOptions<JsonSerializerOptions>? jsonSerializerOptions)
        {
	        _contextAccessor = contextAccessor;
	        _jsonSerializerOptions = jsonSerializerOptions?.Value;
        }

        public HttpClientContextHandler(HttpMessageHandler handler, IContextAccessor<T> contextAccessor, JsonSerializerOptions? jsonSerializerOptions) : base(handler)
        {
	        _contextAccessor = contextAccessor;
	        _jsonSerializerOptions = jsonSerializerOptions;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            SerializeContextToHeaders(request);

            return await base.SendAsync(request, cancellationToken);
        }

        private void SerializeContextToHeaders(HttpRequestMessage request)
        {
	        if (_contextAccessor.Context == null)
		        return;


	        var json = JsonSerializer.Serialize(_contextAccessor.Context, _jsonSerializerOptions);

            request.Headers.Add(AspNetContextAccessor<T>.HeaderKey, json);
        }
    }
}
