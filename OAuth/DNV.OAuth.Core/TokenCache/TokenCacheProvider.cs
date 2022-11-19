using DNV.OAuth.Abstractions;
using Microsoft.AspNetCore.DataProtection;

namespace DNV.OAuth.Core.TokenCache
{
	public class TokenCacheProvider : TokenCacheProviderBase
	{
		protected IDataProtector? DataProtector { get; }
		public bool UseDataProtection { get; set; } = true;

		public TokenCacheProvider(ICacheStorage cacheStorage) : this(cacheStorage, null) { }

		public TokenCacheProvider(ICacheStorage cacheStorage, IDataProtectionProvider? dataProtectionProvider) : base(cacheStorage)
		{
			DataProtector = dataProtectionProvider?.CreateProtector(nameof(TokenCacheProvider));
		}

		protected override byte[]? Protect(byte[]? bytes) =>
			bytes != null && UseDataProtection && DataProtector != null ? DataProtector.Protect(bytes) : bytes;

		protected override byte[]? Unprotect(byte[]? bytes) =>
			bytes != null && UseDataProtection && DataProtector != null ? DataProtector.Unprotect(bytes) : bytes;
	}
}