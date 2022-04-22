using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Distributed;

namespace DNV.OAuth.Core.TokenCache
{
	public class TokenCacheProvider : TokenCacheProviderBase
	{
		protected IDataProtector? DataProtector { get; }

		public TokenCacheProvider(IDistributedCache cache, 
			DistributedCacheEntryOptions cacheOptions, 
			IDataProtectionProvider? dataProtectionProvider) : base(cache, cacheOptions)
		{
			DataProtector = dataProtectionProvider?.CreateProtector(nameof(TokenCacheProvider));
		}

		protected override byte[]? Protect(byte[]? bytes) =>
			bytes != null && DataProtector != null ? DataProtector.Protect(bytes) : bytes;

		protected override byte[]? Unprotect(byte[]? bytes) => 
			bytes != null && DataProtector != null ? DataProtector.Unprotect(bytes) : bytes;
	}
}