using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace DNVGL.OAuth.Web.TokenCache
{
	public class MsalTokenCacheProvider : MsalAbstractTokenCacheProvider
	{
		protected readonly IDistributedCache Cache;

		protected readonly DistributedCacheEntryOptions CacheOptions;

		public MsalTokenCacheProvider(IDistributedCache memoryCache, IOptions<DistributedCacheEntryOptions> cacheOptions) : this(memoryCache, cacheOptions.Value) { }

		public MsalTokenCacheProvider(IDistributedCache memoryCache, DistributedCacheEntryOptions cacheOptions = null)
		{
			if (cacheOptions == null)
			{
				cacheOptions = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60) };
			}

			Cache = memoryCache;
			CacheOptions = cacheOptions;
		}

		protected override Task RemoveKeyAsync(string cacheKey)
		{
			Cache.Remove(cacheKey);
			return Task.CompletedTask;
		}

		protected override Task<byte[]> ReadCacheBytesAsync(string cacheKey)
		{
			var tokenCacheBytes = Cache.Get(cacheKey);
			return Task.FromResult(tokenCacheBytes);
		}

		protected override Task WriteCacheBytesAsync(string cacheKey, byte[] bytes)
		{
			Cache.Set(cacheKey, bytes, CacheOptions);
			return Task.CompletedTask;
		}
	}
}