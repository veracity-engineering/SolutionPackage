using DNV.OAuth.Abstractions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace DNV.OAuth.Core.TokenCache
{
	public class DistributedCacheStorage : ICacheStorage
	{
		private readonly IDistributedCache _cache;
		private readonly DistributedCacheEntryOptions _options;

		public DistributedCacheStorage(IDistributedCache cache, IOptions<DistributedCacheEntryOptions> options) : this(cache, options?.Value) { }

		public DistributedCacheStorage(IDistributedCache cache, DistributedCacheEntryOptions? options)
		{
			_cache = cache;
			_options = options ?? new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromHours(8) };
		}

		public byte[] Get(string key) => _cache.Get(key);

		public Task<byte[]?> GetAsync(string key) => _cache.GetAsync(key);

		public void Remove(string key) => _cache.Remove(key);

		public Task RemoveAsync(string key) => _cache.RemoveAsync(key);

		public void Set(string key, byte[]? value) => _cache.Set(key, value, _options);

		public Task SetAsync(string key, byte[]? value) => _cache.SetAsync(key, value, _options);
	}
}
