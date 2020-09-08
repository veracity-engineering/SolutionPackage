using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace DNVGL.OAuth.Web.TokenCache
{
	public class MsalTokenCacheProvider : MsalAbstractTokenCacheProvider
	{
		private readonly IDistributedCache _cache;

		private readonly DistributedCacheEntryOptions _cacheOptions;

		public MsalTokenCacheProvider(IDistributedCache memoryCache, IOptions<DistributedCacheEntryOptions> cacheOptions) : this(memoryCache, cacheOptions.Value) { }

		public MsalTokenCacheProvider(IDistributedCache memoryCache, DistributedCacheEntryOptions cacheOptions = null)
		{
			if (cacheOptions == null)
			{
				cacheOptions = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60) };
			}

			_cache = memoryCache;
			_cacheOptions = cacheOptions;
		}

		protected override Task RemoveKeyAsync(string cacheKey)
		{
			_cache.Remove(cacheKey);
			return Task.CompletedTask;
		}

		protected override Task<byte[]> ReadCacheBytesAsync(string cacheKey)
		{
			var tokenCacheBytes = _cache.Get(cacheKey) as byte[];
			return Task.FromResult(tokenCacheBytes);
		}

		protected override Task WriteCacheBytesAsync(string cacheKey, byte[] bytes)
		{
			_cache.Set(cacheKey, bytes, _cacheOptions);
			return Task.CompletedTask;
		}
	}
}