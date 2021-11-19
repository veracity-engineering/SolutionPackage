﻿using DNVGL.OAuth.Web.Abstractions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Identity.Client;
using System;
using System.Threading.Tasks;

namespace DNVGL.OAuth.Web.TokenCache
{
	public abstract class TokenCacheProviderBase : ITokenCacheProvider
	{
		protected IDistributedCache Cache { get; }
		protected DistributedCacheEntryOptions CacheOptions { get; }

		public TokenCacheProviderBase(IDistributedCache cache, DistributedCacheEntryOptions cacheOptions)
		{
			Cache = cache ?? throw new ArgumentNullException(nameof(cache));
			CacheOptions = cacheOptions ?? throw new ArgumentNullException(nameof(cacheOptions));
		}

		public virtual Task InitializeAsync(ITokenCache tokenCache)
		{
			if (tokenCache == null) throw new ArgumentNullException(nameof(tokenCache));

			tokenCache.SetBeforeAccessAsync(this.OnBeforeAccessAsync);
			tokenCache.SetAfterAccessAsync(this.OnAfterAccessAsync);
			tokenCache.SetBeforeWriteAsync(this.OnBeforeWriteAsync);
			return Task.CompletedTask;
		}

		public virtual async Task ClearAsync(string identifier) => await this.RemoveKeyAsync(identifier).ConfigureAwait(false);

		protected virtual async Task OnAfterAccessAsync(TokenCacheNotificationArgs args)
		{
			if (args.HasStateChanged)
			{
				if (args.HasTokens)
				{
					var bytes = this.Protect(args.TokenCache.SerializeMsalV3());
					await this.WriteCacheBytesAsync(args.SuggestedCacheKey, bytes).ConfigureAwait(false);
				}
				else
				{
					await this.RemoveKeyAsync(args.SuggestedCacheKey).ConfigureAwait(false);
				}
			}
		}

		protected virtual async Task OnBeforeAccessAsync(TokenCacheNotificationArgs args)
		{
			if (!string.IsNullOrEmpty(args.SuggestedCacheKey))
			{
				var bytes = await this.ReadCacheBytesAsync(args.SuggestedCacheKey).ConfigureAwait(false);
				args.TokenCache.DeserializeMsalV3(this.Unprotect(bytes), true);
			}
		}

		protected virtual Task OnBeforeWriteAsync(TokenCacheNotificationArgs args) => Task.CompletedTask;

		protected virtual Task RemoveKeyAsync(string cacheKey) => Cache.RemoveAsync(cacheKey);

		protected virtual Task<byte[]> ReadCacheBytesAsync(string cacheKey) => Cache.GetAsync(cacheKey);

		protected virtual Task WriteCacheBytesAsync(string cacheKey, byte[] bytes) => Cache.SetAsync(cacheKey, bytes, CacheOptions);

		protected abstract byte[] Protect(byte[] bytes);

		protected abstract byte[] Unprotect(byte[] bytes);
	}
}
