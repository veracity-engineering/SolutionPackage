using DNVGL.OAuth.Api.HttpClient.HttpClientHandlers;
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

        public OAuthHttpClientFactory(IEnumerable<OAuthHttpClientFactoryOptions> options, IHttpContextAccessor httpContextAccessor)
        {
            _options = options;
            _httpContextAccessor = httpContextAccessor;
        }

        public System.Net.Http.HttpClient Create(string name)
        {
            var config = _options.FirstOrDefault(o => o.Name.Equals(name));
            if (config == null)
                throw new Exception($"No {nameof(OAuthHttpClientFactoryOptions)} could be retrieved where Name = '{name}'.");
            return BuildClient(config);
        }

        private System.Net.Http.HttpClient BuildClient(OAuthHttpClientFactoryOptions options)
        {
            if (options.Flow == OAuthCredentialFlow.UserCredentials)
                return new System.Net.Http.HttpClient(new UserCredentialsClientHandler(options, _httpContextAccessor)) { BaseAddress = new Uri(options.BaseUrl) };
            if (options.Flow == OAuthCredentialFlow.ClientCredentials)
                return new System.Net.Http.HttpClient(new ClientCredentialsClientHandler(options)) { BaseAddress = new Uri(options.BaseUrl) };
            throw new Exception($"Invalid credential flow '{options.Flow}'.");
        }
    }
}
