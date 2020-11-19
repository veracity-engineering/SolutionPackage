using DNVGL.OAuth.Api.HttpClient.HttpClientHandlers;
using DNVGL.OAuth.Web.Abstractions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DNVGL.OAuth.Api.HttpClient
{
    public class OAuthHttpClientFactory : IOAuthHttpClientFactory
    {
        private readonly IEnumerable<OAuthHttpClientFactoryOptions> _options;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IClientAppBuilder _appBuilder;

        public OAuthHttpClientFactory(IEnumerable<OAuthHttpClientFactoryOptions> options, IHttpContextAccessor httpContextAccessor, IClientAppBuilder appBuilder)
        {
            _options = options;
            _httpContextAccessor = httpContextAccessor;
            _appBuilder = appBuilder;
        }

        public System.Net.Http.HttpClient Create(string name)
        {
            var config = _options.FirstOrDefault(o => o.Name.Equals(name));
            if (config == null)
                throw new Exception($"No instance of {nameof(OAuthHttpClientFactoryOptions)} could be retrieved where Name = '{name}'.");
            return BuildClient(config);
        }

        private System.Net.Http.HttpClient BuildClient(OAuthHttpClientFactoryOptions options)
        {
            var handlers = new Dictionary<OAuthCredentialFlow, Func<OAuthHttpClientFactoryOptions, System.Net.Http.HttpMessageHandler>>
            {
                { OAuthCredentialFlow.UserCredentials, o => new UserCredentialsHandler(o, _httpContextAccessor, _appBuilder) },
                { OAuthCredentialFlow.ClientCredentials, o => new ClientCredentialsHandler(o, _appBuilder) }
            };
            if (handlers.ContainsKey(options.Flow))
                return new System.Net.Http.HttpClient(handlers[options.Flow](options)) { BaseAddress = new Uri(options.BaseUri) };
            throw new Exception($"Invalid credential flow '{options.Flow}'.");
        }
    }
}
