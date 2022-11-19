using DNV.OAuth.Abstractions;
using DNV.OAuth.Core.Exceptions;
using Microsoft.Identity.Client;
using System;

namespace DNV.OAuth.Core
{
	/// <summary>
	/// Provides a simple way to create <see cref="IClientApp"/>.
	/// </summary>
	public class MsalClientAppBuilder : IClientAppBuilder
	{
		private readonly ITokenCacheProvider _tokenCacheProvider;

		public MsalClientAppBuilder(ITokenCacheProvider? tokenCacheProvider)
		{
			_tokenCacheProvider = tokenCacheProvider;
		}

		/// <summary>
		/// Builds a <see cref="IClientApp"/> instance with giving <see cref="OAuth2Options"/>.
		/// </summary>
		/// <param name="options"></param>
		/// <returns></returns>
		public IClientApp Build(OAuth2Options options)
		{
			if (string.IsNullOrWhiteSpace(options.Scope))
				throw new MissingScopeException();

			var builder = ConfidentialClientApplicationBuilder.Create(options.ClientId)
				.WithAuthority(new Uri(options.Authority));

			if (!string.IsNullOrWhiteSpace(options.ClientSecret))
				builder.WithClientSecret(options.ClientSecret);

			var clientApp = builder.Build();

			_tokenCacheProvider?.InitializeAsync(clientApp.UserTokenCache);
			_tokenCacheProvider?.InitializeAsync(clientApp.AppTokenCache);

			return new MsalClientApp(clientApp, options.Scope);
		}
	}
}
