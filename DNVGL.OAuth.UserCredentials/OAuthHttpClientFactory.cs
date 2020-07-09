using DNVGL.OAuth.UserCredentials.HttpClientHandlers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace DNVGL.OAuth.UserCredentials
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

        public HttpClient Create(string name)
        {
            var config = _options.FirstOrDefault(o => o.Name.Equals(name));
            if (config == null)
                throw new Exception($"No {nameof(OAuthHttpClientFactoryOptions)} could be retrieved where Name = '{name}'.");
            return BuildClient(config);
        }

        private HttpClient BuildClient(OAuthHttpClientFactoryOptions options)
        {
            if (options.Flow == OAuthCredentialFlow.UserCredentials)
                return new HttpClient(new UserCredentialsClientHandler(options, _httpContextAccessor)) { BaseAddress = new Uri(options.BaseUrl) };
            if (options.Flow == OAuthCredentialFlow.ClientCredentials)
                return new HttpClient(new ClientCredentialsClientHandler(options)) { BaseAddress = new Uri(options.BaseUrl) };
            throw new Exception($"Invalid credential flow '{options.Flow}'.");
        }
    }
}
