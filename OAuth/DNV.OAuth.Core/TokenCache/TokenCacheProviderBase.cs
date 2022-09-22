using DNV.OAuth.Abstractions;
using Microsoft.Identity.Client;
using System;
using System.Threading.Tasks;

namespace DNV.OAuth.Core.TokenCache
{
	public abstract class TokenCacheProviderBase : ITokenCacheProvider
	{
		protected ICacheStorage CacheStorage { get; private set; }

		public TokenCacheProviderBase(ICacheStorage cacheStorage)
		{
			this.CacheStorage = cacheStorage;
		}

		public virtual Task InitializeAsync(ITokenCache tokenCache)
		{
			if (tokenCache == null) throw new ArgumentNullException(nameof(tokenCache));

			tokenCache.SetBeforeAccessAsync(OnBeforeAccessAsync);
			tokenCache.SetAfterAccessAsync(OnAfterAccessAsync);
			tokenCache.SetBeforeWriteAsync(OnBeforeWriteAsync);
			return Task.CompletedTask;
		}

		public virtual async Task ClearAsync(string identifier) => await this.CacheStorage.RemoveAsync(identifier).ConfigureAwait(false);

		protected virtual async Task OnAfterAccessAsync(TokenCacheNotificationArgs args)
		{
			if (args.HasStateChanged)
			{
				if (args.HasTokens)
				{
					var bytes = Protect(args.TokenCache.SerializeMsalV3());
					await CacheStorage.SetAsync(args.SuggestedCacheKey, bytes).ConfigureAwait(false);
				}
				else
				{
					await CacheStorage.RemoveAsync(args.SuggestedCacheKey).ConfigureAwait(false);
				}
			}
		}

		protected virtual async Task OnBeforeAccessAsync(TokenCacheNotificationArgs args)
		{
			if (!string.IsNullOrEmpty(args.SuggestedCacheKey))
			{
				var bytes = await CacheStorage.GetAsync(args.SuggestedCacheKey).ConfigureAwait(false);
				args.TokenCache.DeserializeMsalV3(this.Unprotect(bytes), true);
			}
		}
		
		protected virtual Task OnBeforeWriteAsync(TokenCacheNotificationArgs args) => Task.CompletedTask;

		protected abstract byte[]? Protect(byte[]? bytes);

		protected abstract byte[]? Unprotect(byte[]? bytes);
	}
}
