using System;
using System.Linq;
using System.Threading.Tasks;
using DNV.OAuth.Abstractions;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace DNVGL.OAuth.Api.HttpClient.HttpClientHandlers
{
	internal class ClientCredentialsHandler : BaseHttpClientHandler
	{
		private readonly Lazy<IClientApp> _clientApp;
		private readonly Lazy<AuthenticationContext> _authContext;

		public ClientCredentialsHandler(OAuthHttpClientOptions option, IClientAppBuilder appBuilder) : base(option)
		{
			_clientApp = new Lazy<IClientApp>(() => appBuilder.Build(OAuthOptions));
			_authContext = new Lazy<AuthenticationContext>(() => new AuthenticationContext(Authority, new TokenCache()));
		}

		protected override Task<string> RetrieveToken()
		{
			return IsVersion2(_option)
				? GetVersion2AccessToken()
				: GetVersion1AccessToken();
		}

		private static bool IsVersion2(OAuthHttpClientOptions options)
		{
			var uri = new Uri(options.OAuthClientOptions.Authority);
			return uri.Segments.Last().Equals("v2.0", StringComparison.InvariantCultureIgnoreCase);
		}

		private async Task<string> GetVersion2AccessToken()
		{
			var authResult = await _clientApp.Value.AcquireTokenForClient();
			return authResult.AccessToken;
		}

		private async Task<string> GetVersion1AccessToken()
		{
			var authResult = await _authContext.Value.AcquireTokenAsync(_option.OAuthClientOptions.Resource,
				new ClientCredential(_option.OAuthClientOptions.ClientId, _option.OAuthClientOptions.ClientSecret));
			return authResult.AccessToken;
		}
	}
}
