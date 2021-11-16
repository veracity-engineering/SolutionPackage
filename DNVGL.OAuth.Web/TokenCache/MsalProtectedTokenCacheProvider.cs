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

		protected override byte[] Protect(byte[] bytes) =>
			bytes != null ? _dataProtector.Protect(bytes) : null;

		protected override byte[] Unprotect(byte[] bytes) =>
			bytes != null ? _dataProtector.Unprotect(bytes) : null;
	}
}