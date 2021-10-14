using System;
using System.Linq;
using System.Threading.Tasks;
using DNVGL.OAuth.Web.Abstractions;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace DNVGL.OAuth.Api.HttpClient.HttpClientHandlers
{
	internal class ClientCredentialsHandler : BaseHttpClientHandler
	{
		private readonly IClientAppBuilder _appBuilder;
		private IClientApp _clientApp;
		private TokenCache _tokenCache;

		public ClientCredentialsHandler(OAuthHttpClientFactoryOptions options, IClientAppBuilder appBuilder) : base(options)
		{
			_appBuilder = appBuilder ?? throw new ArgumentNullException(nameof(appBuilder));
		}

		protected override Task<string> RetrieveToken()
		{
			return IsVersion2(_options)
				? GetVersion2AccessToken()
				: GetVersion1AccessToken();
		}

		private bool IsVersion2(OAuthHttpClientFactoryOptions options)
		{
			var uri = new Uri(options.OAuthClientOptions.Authority);
			return uri.Segments.Last().Equals("v2.0", StringComparison.InvariantCultureIgnoreCase);
		}

		private async Task<string> GetVersion2AccessToken()
		{
			var clientApp = GetOrCreateClientApp();
			var authResult = await clientApp.AcquireTokenForClient();
			return authResult.AccessToken;
		}

		private async Task<string> GetVersion1AccessToken()
		{
			var authContext = new AuthenticationContext(_options.OAuthClientOptions.Authority, GetTokenCache());
			var authResult = await authContext.AcquireTokenAsync(_options.OAuthClientOptions.Resource, 
                new ClientCredential(_options.OAuthClientOptions.ClientId, _options.OAuthClientOptions.ClientSecret));
			return authResult.AccessToken;
		}

		private IClientApp GetOrCreateClientApp()
		{
			if (_clientApp != null)
				return _clientApp;
			_clientApp = _appBuilder
				.WithOAuth2Options(_options.OAuthClientOptions)
				.BuildForClientCredentials();
			return _clientApp;
		}

		private TokenCache GetTokenCache()
		{
			if (_tokenCache == null)
				_tokenCache = new TokenCache();
			return _tokenCache;
		}
	}
}
