using DNVGL.OAuth.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using System;

namespace DNVGL.OAuth.Demo
{
	public static class MsalAppBuilder
	{
		//public static IServiceCollection AddInMemoryTokenCaches()
		public static IConfidentialClientApplication BuildConfidentialClientApplication(OidcOptions oidcOptions, HttpContext httpContext)
		{
			var clientApp = BuildClientApp(oidcOptions, httpContext);
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

		private static IConfidentialClientApplication BuildClientApp(OidcOptions oidcOptions, HttpContext httpContext)
		{
			var request = httpContext.Request;
			var returnUri = $"{request.Scheme}://{request.Host}{request.PathBase}{oidcOptions.CallbackPath}";
			var clientapp = ConfidentialClientApplicationBuilder.Create(oidcOptions.ClientId)
				.WithAuthority(new Uri(oidcOptions.Authority))
				.WithClientSecret(oidcOptions.ClientSecret)
				.WithRedirectUri(returnUri)
				.Build();

			var cache = httpContext.RequestServices.GetService<IDistributedCache>();
			var provider = new MsalMemoryTokenCacheProvider(cache, new DistributedCacheEntryOptions
			{
				AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60)
			});

			provider.InitializeAsync(clientapp.UserTokenCache);
			//MsalMemoryTokenCacheProvider(cache, clientapp.UserTokenCache);
			return clientapp;
		}
	}
}
