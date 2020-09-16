using DNVGL.OAuth.Web;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Identity.Client;
using System;
using System.Threading.Tasks;

namespace DNVGL.OAuth.Demo.TokenCache
{
	public class MsalAppBuilder
	{
		private OidcOptions _oidcOptions;
		private IMsalTokenCacheProvider _tokenCacheProvider;

		public MsalAppBuilder(OidcOptions oidcOptions, IMsalTokenCacheProvider tokenCacheProvider)
		{
			_oidcOptions = oidcOptions;
			_tokenCacheProvider = tokenCacheProvider;
		}

		public async Task ClearUserTokenCache(HttpContext httpContext)
		{
			var clientApp = this.BuildClientApp(httpContext);
			var userAccount = await clientApp.GetAccountAsync(httpContext.User.GetMsalAccountId());

			if (userAccount != null)
			{
				await clientApp.RemoveAsync(userAccount);
			}
		}

		public async Task<AuthenticationResult> AcquireTokenByAuthorizationCode(AuthorizationCodeReceivedContext context)
		{
			context.HandleCodeRedemption();
			var clientApp = this.BuildClientApp(context.HttpContext, context.TokenEndpointRequest.GetParameter("code_verifier"));
			var result = await clientApp.AcquireTokenByAuthorizationCode(null, context.ProtocolMessage.Code).ExecuteAsync();
			context.HandleCodeRedemption(result.AccessToken, result.IdToken);
			return result;
		}

		public async Task<IAccount> GetAccount(HttpContext httpContext)
		{
			var clientApp = this.BuildClientApp(httpContext);
			var account = await clientApp.GetAccountAsync(httpContext.User.GetMsalAccountId());
			return account;
		}

		private IConfidentialClientApplication BuildClientApp(HttpContext httpContext, string codeVerifier = null)
		{
			var request = httpContext.Request;
			var returnUri = UriHelper.BuildAbsolute(request.Scheme, request.Host, request.PathBase, _oidcOptions.CallbackPath);
			var builder = ConfidentialClientApplicationBuilder.Create(_oidcOptions.ClientId)
				.WithAuthority(new Uri(_oidcOptions.Authority))
				.WithRedirectUri(returnUri);

			if (!string.IsNullOrWhiteSpace(_oidcOptions.ClientSecret))
			{
				builder.WithClientSecret(_oidcOptions.ClientSecret);
			}

			if (!string.IsNullOrWhiteSpace(codeVerifier))
			{
				builder.WithExtraQueryParameters($"code_verifier={codeVerifier}");
			}

			var clientApp = builder.Build();
			
			if(_tokenCacheProvider != null)
			{
				_tokenCacheProvider.InitializeAsync(clientApp.UserTokenCache);
			}

			return clientApp;
		}
	}
}
