﻿using DNVGL.OAuth.Web.Abstractions;
using Microsoft.Identity.Client;
using System;
using System.Threading.Tasks;

namespace DNVGL.OAuth.Web.TokenCache
{
	public abstract class MsalAbstractTokenCacheProvider : ITokenCacheProvider
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

		private Task OnAfterAccessAsync(TokenCacheNotificationArgs args)
		{
			if (args.HasStateChanged)
			{
				return args.HasTokens
					? this.WriteCacheBytesAsync(args.SuggestedCacheKey, args.TokenCache.SerializeMsalV3())
					: this.RemoveKeyAsync(args.SuggestedCacheKey);
			}
			return Task.CompletedTask;
		}

		private async Task OnBeforeAccessAsync(TokenCacheNotificationArgs args)
		{
			if (!string.IsNullOrEmpty(args.SuggestedCacheKey))
			{
				var bytes = await this.ReadCacheBytesAsync(args.SuggestedCacheKey);
				args.TokenCache.DeserializeMsalV3(bytes, true);
			}
		}

		protected virtual Task OnBeforeWriteAsync(TokenCacheNotificationArgs args) => Task.CompletedTask;

		public Task ClearAsync(string identifier)
		{
			// This is a user token cache
			return this.RemoveKeyAsync(identifier);

			// TODO: Clear the cookie session if any. Get inspiration from
			// https://github.com/Azure-Samples/active-directory-aspnetcore-webapp-openidconnect-v2/issues/240
		}

		protected abstract Task WriteCacheBytesAsync(string cacheKey, byte[] bytes);

		protected abstract Task<byte[]> ReadCacheBytesAsync(string cacheKey);

		protected abstract Task RemoveKeyAsync(string cacheKey);
	}
}
