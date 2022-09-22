using DNV.OAuth.Abstractions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace DNV.OAuth.Core.TokenCache
{
	public class MemoryCacheStorage : ICacheStorage
	{
		private readonly IMemoryCache _cache;
		private readonly MemoryCacheEntryOptions _options;

		public MemoryCacheStorage(IMemoryCache cache, IOptions<MemoryCacheEntryOptions> options) : this(cache, options?.Value) { }

		public MemoryCacheStorage(IMemoryCache cache, MemoryCacheEntryOptions? options)
		{
			_cache = cache;
			_options = options ?? new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromHours(8) };
		}

		public byte[]? Get(string key) => _cache.Get(key) as byte[];

		public Task<byte[]?> GetAsync(string key) => Task.FromResult(this.Get(key));

		public void Remove(string key) => _cache.Remove(key);

		public Task RemoveAsync(string key)
		{
			this.Remove(key);
			return Task.CompletedTask;
		}

		public void Set(string key, byte[]? value) => _cache.Set(key, value, _options);

		public Task SetAsync(string key, byte[]? value)
		{
			this.Set(key, value);
			return Task.CompletedTask;
		}
	}
}
