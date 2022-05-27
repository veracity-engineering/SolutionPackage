using DNVGL.OAuth.Api.HttpClient.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace DNVGL.OAuth.Api.HttpClient
{
	[Obsolete("Please use interface System.Net.Http.IHttpClientFactory in BCL instead.")]
	public class OAuthHttpClientFactory : IOAuthHttpClientFactory
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IReadOnlyList<OAuthHttpClientOptions> _options;

		public OAuthHttpClientFactory(IHttpClientFactory httpClientFactory, IEnumerable<OAuthHttpClientOptions> options)
		{
			_httpClientFactory = httpClientFactory;
			_options = options.ToList();
		}

		public IEnumerable<OAuthHttpClientOptions> ClientOptions => _options;

		public System.Net.Http.HttpClient Create(Func<OAuthHttpClientOptions, bool> configPredict, Action<OAuthHttpClientOptions>? configOverride = null)
		{
			var option = ClientOptions.FirstOrDefault(configPredict);

			if (option == null) throw new ClientConfigurationNotFoundException();

			configOverride?.Invoke(option);

			return Create(option);
		}

		public System.Net.Http.HttpClient Create(string apiName)
		{
			var option = ClientOptions.FirstOrDefault(o => apiName == $"{o.Name}:{o.Flow}") ?? ClientOptions.FirstOrDefault(o => apiName == o.Name);

			if (option == null) throw new ClientConfigurationNotFoundException();

			return Create(option);
		}

		private System.Net.Http.HttpClient Create(OAuthHttpClientOptions option)
		{
			return _httpClientFactory.CreateClient($"{option.Name}:{option.Flow}");
		}

		// private readonly Func<OAuthHttpClientFactoryOptions, DelegatingHandler>[] _handlerCreators;
		// private readonly List<OAuthHttpClientFactoryOptions> _option;
		// private readonly IHttpContextAccessor _httpContextAccessor;
		// private readonly IClientAppBuilder _appBuilder;
		//
		// public OAuthHttpClientFactory(IEnumerable<OAuthHttpClientFactoryOptions> options, IHttpContextAccessor httpContextAccessor, IClientAppBuilder appBuilder)
		// {
		//           _option = (options ?? throw new ArgumentNullException(nameof(options))).ToList();
		// 	_httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
		// 	_appBuilder = appBuilder ?? throw new ArgumentNullException(nameof(appBuilder));
		//           _handlerCreators = new Func<OAuthHttpClientFactoryOptions, DelegatingHandler>[Enum.GetValues(typeof(OAuthCredentialFlow)).Length];
		//           _handlerCreators[(int) OAuthCredentialFlow.UserCredentials] =
		//               o => new UserCredentialsHandler(o, _httpContextAccessor, _appBuilder);
		//           _handlerCreators[(int) OAuthCredentialFlow.ClientCredentials] = 
		//               o => new ClientCredentialsHandler(o, _appBuilder);
		//       }
		//
		// public IEnumerable<OAuthHttpClientFactoryOptions> ClientOptions => _option;
		//
		//       public System.Net.Http.HttpClient Create(Func<OAuthHttpClientFactoryOptions, bool> configPredict, Action<OAuthHttpClientFactoryOptions>? configOverride = null)
		//       {
		//           if (configPredict == null)
		//               throw new ArgumentNullException(nameof(configPredict));
		//
		//           var apiConfig = _option.FirstOrDefault(configPredict);
		//           if (apiConfig == null)
		//               throw new ClientConfigurationNotFoundException();
		//
		//           var clonedConfig = CloneConfig(apiConfig);
		//
		//           configOverride?.Invoke(clonedConfig);
		//
		//           return BuildClient(clonedConfig);
		//       }
		//
		//       public System.Net.Http.HttpClient Create(string apiName)
		//       {
		//           if (string.IsNullOrEmpty(apiName))
		//               throw new ArgumentNullException(nameof(apiName));
		//
		//           return Create(c => c.Name == apiName);
		//       }
		//
		//       private System.Net.Http.HttpClient BuildClient(OAuthHttpClientFactoryOptions config)
		//       {
		//        var handler = BuildHandler(config);
		//        handler.InnerHandler = new HttpClientHandler();
		//           return new System.Net.Http.HttpClient(handler) { BaseAddress = new Uri(config.BaseUri) };
		//       }
		//
		//       private static OAuthHttpClientFactoryOptions CloneConfig(OAuthHttpClientFactoryOptions config)
		//       {
		//           return new OAuthHttpClientFactoryOptions
		//           {
		//               Name = config.Name,
		//               Flow = config.Flow,
		//               SubscriptionKey = config.SubscriptionKey,
		//               BaseUri = config.BaseUri,
		//               OAuthClientOptions = new OAuth2Options
		//               {
		//                   ClientId = config.OAuthClientOptions?.ClientId,
		//                   ClientSecret = config.OAuthClientOptions?.ClientSecret,
		//                   Scopes = (string[])(config.OAuthClientOptions?.Scopes.Clone()?? new string[]{}),
		//                   Resource = config.OAuthClientOptions?.Resource,
		//                   Authority = config.OAuthClientOptions?.Authority
		//               }
		//           };
		//       }
		//
		//       public DelegatingHandler BuildHandler(OAuthHttpClientFactoryOptions option)
		//       {
		//           if (option.Flow == OAuthCredentialFlow.ClientCredentials)
		//           {
		//               if (!((option.OAuthClientOptions?.Scopes?.Any() ?? false) || !string.IsNullOrEmpty(option.OAuthClientOptions?.Resource)))
		//                   throw new ArgumentException($"API:({option.Name}) is missing either {nameof(option.OAuthClientOptions.Scopes)} or {nameof(option.OAuthClientOptions.Resource)} value.", $"{nameof(option.OAuthClientOptions)}.{nameof(option.OAuthClientOptions.Scopes)}");
		//
		//               if (string.IsNullOrEmpty(option.OAuthClientOptions?.ClientId))
		//                   throw new ArgumentException($"API:({option.Name}) is missing {nameof(option.OAuthClientOptions.ClientId)} value.",
		//                       $"{nameof(option.OAuthClientOptions)}.{nameof(option.OAuthClientOptions.ClientId)}");
		//
		//               if (string.IsNullOrEmpty(option.OAuthClientOptions?.ClientSecret))
		//                   throw new ArgumentException($"API:({option.Name}) is missing {nameof(option.OAuthClientOptions.ClientSecret)} value.",
		//                       $"{nameof(option.OAuthClientOptions)}.{nameof(option.OAuthClientOptions.ClientSecret)}");
		//
		//               if (string.IsNullOrEmpty(option.OAuthClientOptions?.Authority))
		//                   throw new ArgumentException($"API:({option.Name}) is missing {nameof(option.OAuthClientOptions.Authority)} value.",
		//                       $"{nameof(option.OAuthClientOptions)}.{nameof(option.OAuthClientOptions.Authority)}");
		//           }
		//           else if (!(option.OAuthClientOptions?.Scopes?.Any() ?? false))
		//               throw new ArgumentException($"API:({option.Name}) is missing {nameof(option.OAuthClientOptions.Scopes)} value.", $"{nameof(option.OAuthClientOptions)}.{nameof(option.OAuthClientOptions.Scopes)}");
		//
		//           var creator = _handlerCreators.ElementAtOrDefault((int)option.Flow);
		//
		//           if (creator == null)
		//               throw new InvalidCredentialFlowException(option.Flow);
		//
		//           return creator(option);
		//       }
	}
}
