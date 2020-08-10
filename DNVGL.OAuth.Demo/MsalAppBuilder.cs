using DNVGL.OAuth.Demo.TokenCache;
using DNVGL.OAuth.Web;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using System;

namespace DNVGL.OAuth.Demo
{
	public static class MsalAppBuilder
	{
		public static IConfidentialClientApplication BuildConfidentialClientApplication(OidcOptions oidcOptions, HttpContext httpContext, string codeVerifier = null)
		{
			var clientApp = BuildClientApp(oidcOptions, httpContext, codeVerifier);
			return clientApp;
		}

		//public static async Task ClearUserTokenCache(OidcOptions oidcOptions, HttpContext httpContext)
		//{
		//	var clientApp = BuildClientApp(oidcOptions, httpContext);
		//	var userAccount = await clientApp.GetAccountAsync(ClaimsPrincipal.Current.GetMsalAccountId());

		//	if (userAccount != null)
		//	{
		//		await clientApp.RemoveAsync(userAccount);
		//	}
		//}

		private static IConfidentialClientApplication BuildClientApp(OidcOptions oidcOptions, HttpContext httpContext, string codeVerifier)
		{
			var request = httpContext.Request;
			var returnUri = $"{request.Scheme}://{request.Host}{request.PathBase}{oidcOptions.CallbackPath}";
			var builder = ConfidentialClientApplicationBuilder.Create(oidcOptions.ClientId)
				.WithAuthority(new Uri(oidcOptions.Authority))
				.WithClientSecret(oidcOptions.ClientSecret)
				.WithRedirectUri(returnUri);

			if (!string.IsNullOrWhiteSpace(codeVerifier))
			{
				builder.WithExtraQueryParameters($"code_verifier={codeVerifier}");
			}

			var clientApp = builder.Build();
			var cache = httpContext.RequestServices.GetService<IDistributedCache>();
			var provider = new MsalMemoryTokenCacheProvider(cache, new DistributedCacheEntryOptions
			{
				AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60)
			});
			provider.InitializeAsync(clientApp.UserTokenCache);
			return clientApp;
		}
	}
}
