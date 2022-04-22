using System;
using System.Threading.Tasks;
using DNV.OAuth.Abstractions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Identity.Client;

namespace DNV.OAuth.Core.TokenCache
{
	public abstract class TokenCacheProviderBase : ITokenCacheProvider
	{
		protected IDistributedCache Cache { get; }

		protected DistributedCacheEntryOptions CacheOptions { get; }

		protected TokenCacheProviderBase(IDistributedCache cache, DistributedCacheEntryOptions cacheOptions)
		{;
			Cache = cache;
			CacheOptions = cacheOptions;
		}

		public virtual Task InitializeAsync(ITokenCache tokenCache)
		{
			if (tokenCache == null) throw new ArgumentNullException(nameof(tokenCache));

			tokenCache.SetBeforeAccessAsync(this.OnBeforeAccessAsync);
			tokenCache.SetAfterAccessAsync(this.OnAfterAccessAsync);
			tokenCache.SetBeforeWriteAsync(this.OnBeforeWriteAsync);
			return Task.CompletedTask;
		}

		public virtual async Task ClearAsync(string identifier) => await this.Cache.RemoveAsync(identifier).ConfigureAwait(false);

		protected virtual async Task OnAfterAccessAsync(TokenCacheNotificationArgs args)
		{
			if (args.HasStateChanged)
			{
				if (args.HasTokens)
				{
					var bytes = this.Protect(args.TokenCache.SerializeMsalV3());
					await this.Cache.SetAsync(args.SuggestedCacheKey, bytes, this.CacheOptions).ConfigureAwait(false);
				}
				else
				{
					await Cache.RemoveAsync(args.SuggestedCacheKey).ConfigureAwait(false);
				}
			}
		}

		protected virtual async Task OnBeforeAccessAsync(TokenCacheNotificationArgs args)
		{
			if (!string.IsNullOrEmpty(args.SuggestedCacheKey))
			{
				var bytes = await this.Cache.GetAsync(args.SuggestedCacheKey).ConfigureAwait(false);
				args.TokenCache.DeserializeMsalV3(this.Unprotect(bytes), true);
			}
		}

		protected virtual Task OnBeforeWriteAsync(TokenCacheNotificationArgs args) => Task.CompletedTask;

		protected abstract byte[]? Protect(byte[]? bytes);

		protected abstract byte[]? Unprotect(byte[]? bytes);
	}
}
