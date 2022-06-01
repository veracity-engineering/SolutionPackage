using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace DNV.OAuth.Core.TokenCache
{
	public class TokenCacheProvider : TokenCacheProviderBase
	{
		protected IDataProtector? DataProtector { get; }

		public TokenCacheProvider(IDistributedCache cache,
            IOptionsMonitor<DistributedCacheEntryOptions> cacheOptions) : this(cache, cacheOptions, null) { }
		
		public TokenCacheProvider(IDistributedCache cache, 
			IOptionsMonitor<DistributedCacheEntryOptions> cacheOptions, 
			IDataProtectionProvider? dataProtectionProvider)
		{
			Cache = cache;
            CacheOptions = cacheOptions.Get(nameof(TokenCacheProvider));
            DataProtector = dataProtectionProvider?.CreateProtector(nameof(TokenCacheProvider));
		}

		protected override IDistributedCache Cache { get; }
		
		protected override DistributedCacheEntryOptions CacheOptions { get; }

		protected override byte[]? Protect(byte[]? bytes) =>
			bytes != null && DataProtector != null ? DataProtector.Protect(bytes) : bytes;

		protected override byte[]? Unprotect(byte[]? bytes) => 
			bytes != null && DataProtector != null ? DataProtector.Unprotect(bytes) : bytes;
	}
}