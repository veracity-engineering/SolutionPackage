using DNV.OAuth.Abstractions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Cache;
using Moq;
using System.Security.Cryptography;
using Xunit;

namespace DNV.OAuth.Core.TokenCache.UnitTests
{
	public class TokenCacheProviderTests
	{
		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public async void TokenCacheProviderTest(bool useDataProtection)
		{
			var cache = new Dictionary<string, byte[]?>();

			var sut = CreateTokenCacheProvider(cache);
			sut.UseDataProtection = useDataProtection;
			Task action() => sut.InitializeAsync(null);
			await Assert.ThrowsAsync<ArgumentNullException>(action);

			var token = RandomNumberGenerator.GetBytes(100);
			var tokenCache = new TestTokenCache { Token = token.ToArray() };
			var identifier = Guid.NewGuid().ToString("d");

			var args = new TokenCacheNotificationArgs(
				tokenCache,
				It.IsAny<string>(),
				It.IsAny<IAccount>(),
				true,
				true,
				identifier,
				true,
				null,
				CancellationToken.None
			);

			await sut.InitializeAsync(tokenCache);
			Assert.NotNull(tokenCache.AsyncAfterAccess);
			Assert.NotNull(tokenCache.AsyncBeforeAccess);
			Assert.NotNull(tokenCache.AsyncBeforeWrite);

			await tokenCache.AsyncAfterAccess(args);
			Assert.Equal(token.Length + 1, cache[identifier]?.Length);

			await tokenCache.AsyncBeforeAccess(args);
			Assert.Equal(token, tokenCache.Token);

			args = new TokenCacheNotificationArgs(
				tokenCache,
				It.IsAny<string>(),
				It.IsAny<IAccount>(),
				true,
				true,
				identifier,
				false,
				null,
				CancellationToken.None
			);
			await tokenCache.AsyncAfterAccess(args);
			Assert.False(cache.ContainsKey(identifier));

			cache.Add(identifier, token);
			await sut.ClearAsync(identifier);
			Assert.False(cache.ContainsKey(identifier));
		}

		private static TokenCacheProvider CreateTokenCacheProvider(Dictionary<string, byte[]?> storage)
		{
			var cacheStorage = new Mock<ICacheStorage>();
			cacheStorage.Setup(m => m.GetAsync(It.IsAny<string>()))
				.Returns<string>(k => Task.FromResult(storage.ContainsKey(k) ? storage[k] : null));
			cacheStorage.Setup(m => m.SetAsync(It.IsAny<string>(), It.IsAny<byte[]?>()))
				.Callback<string, byte[]?>((k, v) => storage[k] = v);
			cacheStorage.Setup(m => m.RemoveAsync(It.IsAny<string>()))
				.Callback<string>(k =>
				{
					if (storage.ContainsKey(k)) storage.Remove(k);
				});

			var dataProtector = new Mock<IDataProtector>();
			dataProtector.Setup(m => m.Protect(It.IsAny<byte[]>()))
				.Returns<byte[]>(d => d.Reverse().ToArray());
			dataProtector.Setup(m => m.Unprotect(It.IsAny<byte[]>()))
				.Returns<byte[]>(d => d.Reverse().ToArray());

			var dataProtectProvider = new Mock<IDataProtectionProvider>();
			dataProtectProvider.Setup(m => m.CreateProtector(It.IsAny<string>()))
				.Returns(dataProtector.Object);

			return new TokenCacheProvider(cacheStorage.Object, dataProtector.Object);
		}
	}

	public class TestTokenCache : ITokenCache, ITokenCacheSerializer
	{
		public byte[]? Token { get; set; }
		public Func<TokenCacheNotificationArgs, Task>? AsyncBeforeAccess { get; set; }
		public Func<TokenCacheNotificationArgs, Task>? AsyncAfterAccess { get; set; }
		public Func<TokenCacheNotificationArgs, Task>? AsyncBeforeWrite { get; set; }

		public void SetAfterAccessAsync(Func<TokenCacheNotificationArgs, Task> afterAccess)
		{
			this.AsyncAfterAccess = afterAccess;
		}

		public void SetBeforeAccessAsync(Func<TokenCacheNotificationArgs, Task> beforeAccess)
		{
			this.AsyncBeforeAccess = beforeAccess;
		}

		public void SetBeforeWriteAsync(Func<TokenCacheNotificationArgs, Task> beforeWrite)
		{
			this.AsyncBeforeWrite = beforeWrite;
		}

		public byte[]? SerializeMsalV3()
		{
			this.Token = this.Token?.Concat(new byte[] { 1 }).ToArray();
			return this.Token;
		}

		public void DeserializeMsalV3(byte[] msalV3State, bool shouldClearExistingCache = false)
		{
			this.Token = msalV3State?.Take(msalV3State.Length - 1).ToArray();
		}

		#region not implemented
		public void Deserialize(byte[] msalV2State)
		{
			throw new NotImplementedException("This is removed in MSAL.NET v4. Read more: https://aka.ms/msal-net-4x-cache-breaking-change");
		}

		public void DeserializeAdalV3(byte[] adalV3State)
		{
			throw new NotImplementedException("This is removed in MSAL.NET v4. Read more: https://aka.ms/msal-net-4x-cache-breaking-change");
		}

		public void DeserializeMsalV2(byte[] msalV2State)
		{
			throw new NotImplementedException("This is removed in MSAL.NET v4. Read more: https://aka.ms/msal-net-4x-cache-breaking-change");
		}

		public void DeserializeUnifiedAndAdalCache(CacheData cacheData)
		{
			throw new NotImplementedException("This is removed in MSAL.NET v4. Read more: https://aka.ms/msal-net-4x-cache-breaking-change");
		}

		public byte[] Serialize()
		{
			throw new NotImplementedException("This is removed in MSAL.NET v4. Read more: https://aka.ms/msal-net-4x-cache-breaking-change");
		}

		public byte[] SerializeAdalV3()
		{
			throw new NotImplementedException("This is removed in MSAL.NET v4. Read more: https://aka.ms/msal-net-4x-cache-breaking-change");
		}

		public byte[] SerializeMsalV2()
		{
			throw new NotImplementedException("This is removed in MSAL.NET v4. Read more: https://aka.ms/msal-net-4x-cache-breaking-change");
		}

		public CacheData SerializeUnifiedAndAdalCache()
		{
			throw new NotImplementedException("This is removed in MSAL.NET v4. Read more: https://aka.ms/msal-net-4x-cache-breaking-change");
		}
		public void SetAfterAccess(TokenCacheCallback afterAccess)
		{
			throw new NotImplementedException();
		}

		public void SetBeforeAccess(TokenCacheCallback beforeAccess)
		{
			throw new NotImplementedException();
		}

		public void SetBeforeWrite(TokenCacheCallback beforeWrite)
		{
			throw new NotImplementedException();
		}
		#endregion not implemented

	}
}