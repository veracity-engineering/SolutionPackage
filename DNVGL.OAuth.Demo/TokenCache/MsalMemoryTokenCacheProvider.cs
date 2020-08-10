using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace DNVGL.OAuth.Demo
{
	public class MsalMemoryTokenCacheProvider : MsalAbstractTokenCacheProvider
	{
		private readonly IDistributedCache _memoryCache;

		private readonly DistributedCacheEntryOptions _cacheOptions;

		public MsalMemoryTokenCacheProvider(IDistributedCache memoryCache, IOptions<DistributedCacheEntryOptions> cacheOptions) : this(memoryCache, cacheOptions.Value) { }

		public MsalMemoryTokenCacheProvider(IDistributedCache memoryCache, DistributedCacheEntryOptions cacheOptions)
		{
			if (cacheOptions == null)
			{
				throw new ArgumentNullException(nameof(cacheOptions));
			}

			_memoryCache = memoryCache;
			_cacheOptions = cacheOptions;
		}

		protected override Task RemoveKeyAsync(string cacheKey)
		{
			_memoryCache.Remove(cacheKey);
			return Task.CompletedTask;
		}

		protected override Task<byte[]> ReadCacheBytesAsync(string cacheKey)
		{
			var tokenCacheBytes = _memoryCache.Get(cacheKey) as byte[];
			return Task.FromResult(tokenCacheBytes);
		}

		protected override Task WriteCacheBytesAsync(string cacheKey, byte[] bytes)
		{
			_memoryCache.Set(cacheKey, bytes, _cacheOptions);
			return Task.CompletedTask;
		}
	}
}