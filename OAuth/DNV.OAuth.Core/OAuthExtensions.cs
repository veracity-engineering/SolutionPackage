using DNV.OAuth.Abstractions;
using DNV.OAuth.Core.TokenCache;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Security.Claims;

namespace DNV.OAuth.Core
{
	public static class OAuthExtensions
	{
		private const string ObjectId = "http://schemas.microsoft.com/identity/claims/objectidentifier";
		private const string Policy = "http://schemas.microsoft.com/claims/authnclassreference";

		/// <summary>
		/// Generates a MSAL Account Id from user claims and OIDC Options.
		/// </summary>
		/// <param name="claimsPrincipal"></param>
		/// <returns></returns>
		public static string GetMsalAccountId(this ClaimsPrincipal claimsPrincipal)
		{
			var objectId = claimsPrincipal.FindFirst(ObjectId);
			var policy = claimsPrincipal.FindFirst(Policy).Value;
			var tenantId = objectId.Issuer.Split('/')[3];
			var msalAccountId = $"{objectId.Value}-{policy}.{tenantId}";
			return msalAccountId.ToLower();
		}

		/// <summary>
		/// Register OAuthCore required services to DI container.
		/// </summary>
		/// <param name="services"></param>
		public static IServiceCollection AddOAuthCore(this IServiceCollection services)
		{
			services.TryAddSingleton<IClientAppBuilder>(p =>
			{
				var tokenCacheProvider = p.GetService<ITokenCacheProvider>();
				return new MsalClientAppBuilder(tokenCacheProvider);
			});
			return services;
		}

		/// <summary>
		/// Adds in memory token caches
		/// </summary>
		/// <param name="services"></param>
		/// <param name="cacheConfigAction"></param>
		/// <param name="useDataProtection"></param>
		/// <returns></returns>
		public static IServiceCollection AddInMemoryTokenCaches(this IServiceCollection services, Action<MemoryCacheEntryOptions>? cacheConfigAction = null, bool useDataProtection = true)
		{
			if (cacheConfigAction != null) services.Configure(cacheConfigAction);

			services.AddMemoryCache();
			services.TryAddSingleton<ICacheStorage, MemoryCacheStorage>();
			services.AddTokenCaches(useDataProtection);
			return services;
		}

		/// <summary>
		/// Adds distributed token caches
		/// </summary>
		/// <param name="services"></param>
		/// <param name="cacheConfigAction"></param>
		/// <param name="useDataProtection"></param>
		/// <returns></returns>
		public static IServiceCollection AddDistributedTokenCaches(this IServiceCollection services, Action<DistributedCacheEntryOptions>? cacheConfigAction = null, bool useDataProtection = true)
		{
			if (cacheConfigAction != null) services.Configure(cacheConfigAction);

			services.AddDistributedMemoryCache();
			services.TryAddSingleton<ICacheStorage, DistributedCacheStorage>();
			services.AddTokenCaches(useDataProtection);
			return services;
		}

		public static void AddTokenCaches(this IServiceCollection services, bool useDataProtection = true)
		{
			services.TryAddSingleton<ITokenCacheProvider>(p =>
			{
				var tokenCacheProvider = ActivatorUtilities.CreateInstance<TokenCacheProvider>(p);
				tokenCacheProvider.UseDataProtection = useDataProtection;
				return tokenCacheProvider;
			});
		}
	}
}
