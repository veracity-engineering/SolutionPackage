using DNVGL.OAuth.Web.Abstractions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using System;
using System.Threading.Tasks;

namespace DNVGL.OAuth.Web.TokenCache
{
	public class MsalTokenCacheProvider : ITokenCacheProvider
	{
		private readonly IDistributedCache _cache;
		private readonly DistributedCacheEntryOptions _cacheOptions;
		private readonly IDataProtector _dataProtector;

		public MsalTokenCacheProvider(IDistributedCache cache, IServiceProvider serviceProvider, DistributedCacheEntryOptions cacheOptions = null)
		{
			_cache = cache;
			_cacheOptions = cacheOptions ?? new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60) };
			_dataProtector = serviceProvider.GetService<IDataProtector>();
		}

		public Task InitializeAsync(ITokenCache tokenCache)
		{
			if (tokenCache == null) throw new ArgumentNullException(nameof(tokenCache));

			tokenCache.SetBeforeAccessAsync(this.OnBeforeAccessAsync);
			tokenCache.SetAfterAccessAsync(this.OnAfterAccessAsync);
			tokenCache.SetBeforeWriteAsync(this.OnBeforeWriteAsync);
			return Task.CompletedTask;
		}

		public async Task ClearAsync(string identifier)
		{
			// This is a user token cache
			await this.RemoveKeyAsync(identifier).ConfigureAwait(false);

			// TODO: Clear the cookie session if any. Get inspiration from
			// https://github.com/Azure-Samples/active-directory-aspnetcore-webapp-openidconnect-v2/issues/240
		}

		private async Task OnBeforeAccessAsync(TokenCacheNotificationArgs args)
		{
			if (!string.IsNullOrEmpty(args.SuggestedCacheKey))
			{
				var bytes = await this.ReadCacheBytesAsync(args.SuggestedCacheKey).ConfigureAwait(false);

				if (bytes != null && _dataProtector != null) bytes = _dataProtector.Unprotect(bytes);

				args.TokenCache.DeserializeMsalV3(bytes, true);
			}
		}

		private async Task OnAfterAccessAsync(TokenCacheNotificationArgs args)
		{
			if (args.HasStateChanged)
			{
				if (args.HasTokens)
				{
					var bytes = args.TokenCache.SerializeMsalV3();

					if (bytes != null && _dataProtector != null) bytes = _dataProtector.Protect(bytes);

					await this.WriteCacheBytesAsync(args.SuggestedCacheKey, bytes).ConfigureAwait(false);
				}
				else
				{
					await this.RemoveKeyAsync(args.SuggestedCacheKey).ConfigureAwait(false);
				}
			}
		}

		private Task OnBeforeWriteAsync(TokenCacheNotificationArgs args) => Task.CompletedTask;

		private Task RemoveKeyAsync(string cacheKey) => _cache.RemoveAsync(cacheKey);

		private Task<byte[]> ReadCacheBytesAsync(string cacheKey) => _cache.GetAsync(cacheKey);

		private Task WriteCacheBytesAsync(string cacheKey, byte[] bytes) => _cache.SetAsync(cacheKey, bytes, _cacheOptions);
	}
}