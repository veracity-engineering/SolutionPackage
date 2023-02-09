using System;
using System.Threading.Tasks;
using DNV.OAuth.Abstractions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Identity.Client;

namespace DNV.OAuth.Core.TokenCache
{
	public abstract class TokenCacheProviderBase : ITokenCacheProvider
	{
		public virtual Task InitializeAsync(ITokenCache tokenCache)
		{
			if (tokenCache == null) throw new ArgumentNullException(nameof(tokenCache));

			tokenCache.SetBeforeAccessAsync(OnBeforeAccessAsync);
			tokenCache.SetAfterAccessAsync(OnAfterAccessAsync);
			tokenCache.SetBeforeWriteAsync(OnBeforeWriteAsync);
			return Task.CompletedTask;
		}

		public virtual async Task ClearAsync(string identifier) => await this.Cache.RemoveAsync(identifier).ConfigureAwait(false);

		protected virtual async Task OnAfterAccessAsync(TokenCacheNotificationArgs args)
		{
			if (args.HasStateChanged)
			{
				if (args.HasTokens)
				{
					var bytes = Protect(args.TokenCache.SerializeMsalV3());
					await Cache.SetAsync(args.SuggestedCacheKey, bytes, CacheOptions).ConfigureAwait(false);
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
				var bytes = await Cache.GetAsync(args.SuggestedCacheKey).ConfigureAwait(false);
				args.TokenCache.DeserializeMsalV3(this.Unprotect(bytes), true);
			}
		}
		
		protected abstract IDistributedCache Cache { get; }

		protected abstract DistributedCacheEntryOptions CacheOptions { get; }
		
		protected virtual Task OnBeforeWriteAsync(TokenCacheNotificationArgs args) => Task.CompletedTask;

		protected abstract byte[]? Protect(byte[]? bytes);

		protected abstract byte[]? Unprotect(byte[]? bytes);
	}
}
