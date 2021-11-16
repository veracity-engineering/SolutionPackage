using DNVGL.OAuth.Web.Abstractions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Identity.Client;
using System;
using System.Threading.Tasks;

namespace DNVGL.OAuth.Web.TokenCache
{
	/// <summary>
	/// An implementation of token cache for confidential clients backed by <see cref="IDistributedCache"/>.
	/// </summary>
	public class MsalTokenCacheProvider : ITokenCacheProvider
	{
		private readonly IDistributedCache _cache;
		private readonly DistributedCacheEntryOptions _cacheOptions;
		private readonly IDataProtector _dataProtector;

		public MsalTokenCacheProvider(IDistributedCache cache, DistributedCacheEntryOptions cacheOptions, IDataProtectionProvider dataProtectionProvider)
		{
			_cache = cache ?? throw new ArgumentNullException(nameof(cache));
			_cacheOptions = cacheOptions ?? throw new ArgumentNullException(nameof(cacheOptions));
			_dataProtector = dataProtectionProvider?.CreateProtector(nameof(MsalTokenCacheProvider));
		}

		public Task InitializeAsync(ITokenCache tokenCache)
		{
			if (tokenCache == null) throw new ArgumentNullException(nameof(tokenCache));

			tokenCache.SetBeforeAccessAsync(this.OnBeforeAccessAsync);
			tokenCache.SetAfterAccessAsync(this.OnAfterAccessAsync);
			tokenCache.SetBeforeWriteAsync(this.OnBeforeWriteAsync);
			return Task.CompletedTask;
		}

		public async Task ClearAsync(string identifier)
		{
			// This is a user token cache
			await this.RemoveKeyAsync(identifier).ConfigureAwait(false);

			// TODO: Clear the cookie session if any. Get inspiration from
			// https://github.com/Azure-Samples/active-directory-aspnetcore-webapp-openidconnect-v2/issues/240
		}

		private async Task OnBeforeAccessAsync(TokenCacheNotificationArgs args)
		{
			if (!string.IsNullOrEmpty(args.SuggestedCacheKey))
			{
				var bytes = await this.ReadCacheBytesAsync(args.SuggestedCacheKey).ConfigureAwait(false);
				args.TokenCache.DeserializeMsalV3(this.Unprotect(bytes), true);
			}
		}

		private async Task OnAfterAccessAsync(TokenCacheNotificationArgs args)
		{
			if (args.HasStateChanged)
			{
				if (args.HasTokens)
				{
					var bytes = args.TokenCache.SerializeMsalV3();
					await this.WriteCacheBytesAsync(args.SuggestedCacheKey, this.Protect(bytes)).ConfigureAwait(false);
				}
				else
				{
					await this.RemoveKeyAsync(args.SuggestedCacheKey).ConfigureAwait(false);
				}
			}
		}

		private Task OnBeforeWriteAsync(TokenCacheNotificationArgs args) => Task.CompletedTask;

		private Task RemoveKeyAsync(string cacheKey) => _cache.RemoveAsync(cacheKey);

		private Task<byte[]> ReadCacheBytesAsync(string cacheKey) => _cache.GetAsync(cacheKey);

		private Task WriteCacheBytesAsync(string cacheKey, byte[] bytes) => _cache.SetAsync(cacheKey, bytes, _cacheOptions);

		private byte[] Protect(byte[] bytes) =>
			bytes != null && _dataProtector != null ? _dataProtector.Protect(bytes) : bytes;

		private byte[] Unprotect(byte[] bytes) =>
			bytes != null && _dataProtector != null ? _dataProtector.Unprotect(bytes) : bytes;
	}
}