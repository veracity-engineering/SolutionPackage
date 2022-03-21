using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DNV.Context.Abstractions;
using DNV.Context.AspNet;
using Newtonsoft.Json;

namespace DNV.Context.HttpClient
{
    internal class HttpClientContextHandler<T> : DelegatingHandler where T : class
    {
	    public static readonly string ClientName = typeof(T).FullName;

        private readonly IContextAccessor<T> _contextAccessor;
        private readonly JsonSerializerSettings? _jsonSerializerSettings;

        public HttpClientContextHandler(IContextAccessor<T> contextAccessor, JsonSerializerSettings? jsonSerializerSettings)
        {
	        _contextAccessor = contextAccessor;
	        _jsonSerializerSettings = jsonSerializerSettings;
        }

        public HttpClientContextHandler(HttpMessageHandler handler, IContextAccessor<T> contextAccessor, JsonSerializerSettings? jsonSerializerSettings) : base(handler)
        {
	        _contextAccessor = contextAccessor;
	        _jsonSerializerSettings = jsonSerializerSettings;
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

	        var serializer = JsonSerializer.CreateDefault(_jsonSerializerSettings);
            var sb = new StringBuilder();
	        using var sr = new StringWriter(sb);
	        using var jr = new JsonTextWriter(sr);
	        serializer.Serialize(jr, _contextAccessor.Context);

            request.Headers.Add(AspNetContextAccessor<T>.HeaderKey, sb.ToString());
        }
    }
}
