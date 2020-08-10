using Microsoft.Identity.Client;
using Microsoft.Identity.Client.TokenCacheProviders;
using System;
using System.Threading.Tasks;

namespace DNVGL.OAuth.Demo.TokenCache
{
	public abstract class MsalAbstractTokenCacheProvider : IMsalTokenCacheProvider
	{
		public Task InitializeAsync(ITokenCache tokenCache)
		{
			if (tokenCache == null)
			{
				throw new ArgumentNullException(nameof(tokenCache));
			}

			tokenCache.SetBeforeAccessAsync(this.OnBeforeAccessAsync);
			tokenCache.SetAfterAccessAsync(this.OnAfterAccessAsync);
			tokenCache.SetBeforeWriteAsync(this.OnBeforeWriteAsync);
			return Task.CompletedTask;
		}

		private async Task OnAfterAccessAsync(TokenCacheNotificationArgs args)
		{
			if (args.HasStateChanged)
			{
				if (args.HasTokens)
				{
					await this.WriteCacheBytesAsync(args.SuggestedCacheKey, args.TokenCache.SerializeMsalV3()).ConfigureAwait(false);
				}
				else
				{
					await this.RemoveKeyAsync(args.SuggestedCacheKey).ConfigureAwait(false);
				}
			}
		}

		private async Task OnBeforeAccessAsync(TokenCacheNotificationArgs args)
		{
			if (!string.IsNullOrEmpty(args.SuggestedCacheKey))
			{
				var bytes = await this.ReadCacheBytesAsync(args.SuggestedCacheKey).ConfigureAwait(false);
				args.TokenCache.DeserializeMsalV3(bytes, true);
			}
		}

		protected virtual Task OnBeforeWriteAsync(TokenCacheNotificationArgs args) => Task.CompletedTask;

		public async Task ClearAsync(string homeAccountId)
		{
			// This is a user token cache
			await RemoveKeyAsync(homeAccountId).ConfigureAwait(false);

			// TODO: Clear the cookie session if any. Get inspiration from
			// https://github.com/Azure-Samples/active-directory-aspnetcore-webapp-openidconnect-v2/issues/240
		}

		protected abstract Task WriteCacheBytesAsync(string cacheKey, byte[] bytes);

		protected abstract Task<byte[]> ReadCacheBytesAsync(string cacheKey);

		protected abstract Task RemoveKeyAsync(string cacheKey);
	}
}
