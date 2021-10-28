using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Distributed;

namespace DNVGL.OAuth.Web.TokenCache
{
	public class MsalProtectedTokenCacheProvider : MsalTokenCacheProvider
	{
		private readonly IDataProtector _dataProtector;

		public MsalProtectedTokenCacheProvider(
			IDistributedCache memoryCache,
			DistributedCacheEntryOptions cacheOptions,
			IDataProtectionProvider dataProtectionProvider)
			: base(memoryCache, cacheOptions)
		{
			_dataProtector = dataProtectionProvider.CreateProtector(nameof(MsalProtectedTokenCacheProvider));
		}

		protected override Task<byte[]> ReadCacheBytesAsync(string cacheKey)
		{
			var tokenCacheBytes = Cache.Get(cacheKey);
			return Task.FromResult(tokenCacheBytes != null ? _dataProtector.Unprotect(tokenCacheBytes) : null);
		}

		protected override Task WriteCacheBytesAsync(string cacheKey, byte[] bytes)
		{
			Cache.Set(cacheKey, bytes != null ? _dataProtector.Protect(bytes) : null, CacheOptions);
			return Task.CompletedTask;
		}
	}
}