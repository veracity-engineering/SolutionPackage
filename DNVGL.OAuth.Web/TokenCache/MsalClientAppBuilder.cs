using DNVGL.OAuth.Web.Abstractions;
using Microsoft.Identity.Client;
using System;
using System.Linq;

namespace DNVGL.OAuth.Web.TokenCache
{
	/// <summary>
	/// Provides a simple way to create <see cref="IClientApp"/>.
	/// </summary>
	public class MsalClientAppBuilder : IClientAppBuilder
	{
		private readonly ITokenCacheProvider _tokenCacheProvider;
		private readonly OAuth2Options _options;

		public MsalClientAppBuilder(ITokenCacheProvider tokenCacheProvider, OAuth2Options options)
		{
			_tokenCacheProvider = tokenCacheProvider;
			_options = options;
		}

		/// <summary>
		/// Builds a <see cref="IClientApp"/> instance, new scopes can be attatched.
		/// </summary>
		/// <param name="scopes"></param>
		/// <returns></returns>
		public IClientApp Build(params string[] scopes)
		{
			var options = _options.Clone();
			options.Scopes = scopes?.Any() == true ? scopes : _options.Scopes;
			return this.BuildWithOptions(options);
		}

		/// <summary>
		/// Builds a <see cref="IClientApp"/> instance with giving <see cref="OAuth2Options"/>.
		/// </summary>
		/// <param name="options"></param>
		/// <returns></returns>
		public IClientApp BuildWithOptions(OAuth2Options options)
		{
			var builder = ConfidentialClientApplicationBuilder.Create(options.ClientId)
				.WithAuthority(new Uri(options.Authority));

			if (!string.IsNullOrWhiteSpace(options.ClientSecret))
				builder.WithClientSecret(options.ClientSecret);

			var clientApp = builder.Build();

			if (_tokenCacheProvider != null)
			{
				_tokenCacheProvider.InitializeAsync(clientApp.UserTokenCache);
				_tokenCacheProvider.InitializeAsync(clientApp.AppTokenCache);
			}

			return new MsalClientApp(clientApp, options.Scopes);
		}
	}
}
