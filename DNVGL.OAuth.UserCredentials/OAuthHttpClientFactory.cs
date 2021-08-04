using DNVGL.OAuth.Api.HttpClient.Exceptions;
using DNVGL.OAuth.Api.HttpClient.HttpClientHandlers;
using DNVGL.OAuth.Web.Abstractions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace DNVGL.OAuth.Api.HttpClient
{
	public class OAuthHttpClientFactory : IOAuthHttpClientFactory
    {
        private readonly Func<OAuthHttpClientFactoryOptions, System.Net.Http.HttpMessageHandler>[] _handlerCreators;
		private readonly IEnumerable<OAuthHttpClientFactoryOptions> _options;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IClientAppBuilder _appBuilder;

		public OAuthHttpClientFactory(IEnumerable<OAuthHttpClientFactoryOptions> options, IHttpContextAccessor httpContextAccessor, IClientAppBuilder appBuilder)
		{
            _options = options ?? throw new ArgumentNullException(nameof(options));
			_httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
			_appBuilder = appBuilder ?? throw new ArgumentNullException(nameof(appBuilder));
            _handlerCreators = new Func<OAuthHttpClientFactoryOptions, HttpMessageHandler>[Enum.GetValues(typeof(OAuthCredentialFlow)).Length];
            _handlerCreators[(int) OAuthCredentialFlow.UserCredentials] =
                o => new UserCredentialsHandler(o, _httpContextAccessor, _appBuilder);
            _handlerCreators[(int) OAuthCredentialFlow.ClientCredentials] = 
                o => new ClientCredentialsHandler(o, _appBuilder);
        }

        public System.Net.Http.HttpClient Create(string name, Action<OAuthHttpClientFactoryOptions> configAction = null)
        {
            var config = _options.FirstOrDefault(o => o.Name.Equals(name));
            if (config == null)
                throw new MissingClientConfigurationNameException(name);

            var clone = CloneConfig(config);

            configAction?.Invoke(clone);

            return BuildClient(config);
        }

        private System.Net.Http.HttpClient BuildClient(OAuthHttpClientFactoryOptions config)
        {
            if (!(config.OAuthClientOptions?.Scopes?.Any() ?? false))
                throw new ArgumentException($"API:({config.Name}) is missing {nameof(config.OAuthClientOptions.Scopes)} value.", $"{nameof(config.OAuthClientOptions)}.{nameof(config.OAuthClientOptions.Scopes)}");

            if (config.Flow == OAuthCredentialFlow.ClientCredentials)
            {
                if (string.IsNullOrEmpty(config.OAuthClientOptions?.ClientId))
                    throw new ArgumentException($"API:({config.Name}) is missing {nameof(config.OAuthClientOptions.ClientId)} value.",
                        $"{nameof(config.OAuthClientOptions)}.{nameof(config.OAuthClientOptions.ClientId)}");

                if (string.IsNullOrEmpty(config.OAuthClientOptions?.ClientSecret))
                    throw new ArgumentException($"API:({config.Name}) is missing {nameof(config.OAuthClientOptions.ClientSecret)} value.",
                        $"{nameof(config.OAuthClientOptions)}.{nameof(config.OAuthClientOptions.ClientSecret)}");

                if (string.IsNullOrEmpty(config.OAuthClientOptions?.Authority))
                    throw new ArgumentException($"API:({config.Name}) is missing {nameof(config.OAuthClientOptions.Authority)} value.",
                        $"{nameof(config.OAuthClientOptions)}.{nameof(config.OAuthClientOptions.Authority)}");
            }

            var creator = _handlerCreators.ElementAtOrDefault((int)config.Flow);

            if (creator == null)
                throw new InvalidCredentialFlowException(config.Flow);

            return new System.Net.Http.HttpClient(creator(config)) { BaseAddress = new Uri(config.BaseUri) };
        }

        private static OAuthHttpClientFactoryOptions CloneConfig(OAuthHttpClientFactoryOptions config)
        {
            return new OAuthHttpClientFactoryOptions
            {
                Name = config.Name,
                Flow = config.Flow,
                SubscriptionKey = config.SubscriptionKey,
                BaseUri = config.BaseUri,
                OAuthClientOptions = new OAuth2Options
                {
                    ClientId = config.OAuthClientOptions?.ClientId,
                    ClientSecret = config.OAuthClientOptions?.ClientSecret,
                    Scopes = (string[])config.OAuthClientOptions?.Scopes.Clone(),
                    Authority = config.OAuthClientOptions?.Authority
                }
            };
        }
	}
}
