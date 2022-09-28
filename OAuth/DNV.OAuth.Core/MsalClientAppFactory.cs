using DNV.OAuth.Abstractions;
using DNV.OAuth.Core.Exceptions;
using Microsoft.Identity.Client;
using System;

namespace DNV.OAuth.Core
{
	/// <summary>
	/// Factory class to create <see cref="IClientApp"/>.
	/// </summary>
	public class MsalClientAppFactory : IClientAppFactory
	{
		private readonly VeracityClientOptions _options;
		private readonly ITokenCacheProvider? _tokenCacheProvider;

		public MsalClientAppFactory(VeracityClientOptions? options, ITokenCacheProvider? tokenCacheProvider = null)
		{
			_options = options ?? throw new ArgumentNullException(nameof(options));
			_tokenCacheProvider = tokenCacheProvider;
		}

		/// <summary>
		/// Creates a <see cref="IClientApp"/> instance for user credentials.
		/// </summary>
		/// <param name="scope">A scope uri or an app/client Id</param>
		/// <returns></returns>
		/// <exception cref="MissingScopeException"></exception>
		public IClientApp CreateForUser(string? scope)
		{
			if (string.IsNullOrWhiteSpace(scope)) throw new MissingScopeException();

			var authority = _options.VeracityOptions.B2CAuthorityV2;
			scope = _options.VeracityOptions.GetB2CScope(scope);
			var clientApp = ConfidentialClientApplicationBuilder.Create(_options.ClientId)
				.WithAuthority(new Uri(authority))
				.WithClientSecret(_options.ClientSecret)
				.Build();

			_tokenCacheProvider?.InitializeAsync(clientApp.UserTokenCache);
			return new MsalClientApp(clientApp, scope);
		}

		/// <summary>
		/// Creates a <see cref="IClientApp"/> instance for client credentials.
		/// </summary>
		/// <param name="scope">A scope uri or an app/client Id</param>
		/// <returns></returns>
		/// <exception cref="MissingScopeException"></exception>
		public IClientApp CreateForClient(string? scope)
		{
			if (string.IsNullOrWhiteSpace(scope)) throw new MissingScopeException();

			var authority = _options.VeracityOptions.AADAuthorityV2;
			scope = _options.VeracityOptions.GetAADScope(scope);
			var clientApp = ConfidentialClientApplicationBuilder.Create(_options.ClientId)
				.WithAuthority(new Uri(authority))
				.WithClientSecret(_options.ClientSecret)
				.Build();

			_tokenCacheProvider?.InitializeAsync(clientApp.AppTokenCache);
			return new MsalClientApp(clientApp, scope);
		}
	}
}
