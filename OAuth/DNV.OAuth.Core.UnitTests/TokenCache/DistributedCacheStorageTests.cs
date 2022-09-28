using Xunit;
using DNV.OAuth.Core.TokenCache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using System.Security.Cryptography;
using Microsoft.Extensions.Caching.Distributed;

namespace DNV.OAuth.Core.TokenCache.UnitTests
{
	public class DistributedCacheStorageTests
	{
		[Fact()]
		public async void DistributedCacheStorageTest()
		{
			var (key1, value1) = ("item1", RandomNumberGenerator.GetBytes(10));
			var (key2, value2) = ("item2", RandomNumberGenerator.GetBytes(10));

			var sut = CreateSUT(true);
			Assert.NotNull(sut);

			sut = CreateSUT(false);
			sut.Set(key1, value2);
			var value = sut.Get(key1);
			Assert.Equal(value2, value);

			await sut.SetAsync(key2, value1);
			value = await sut.GetAsync(key2);
			Assert.Equal(value1, value);

			sut.Remove(key1);
			value = sut.Get(key1);
			Assert.Null(value);

			await sut.RemoveAsync(key2);
			value = await sut.GetAsync(key2);
			Assert.Null(value);
		}

		private static DistributedCacheStorage CreateSUT(bool withOptions)
		{
			var cacheOptions = new Mock<IOptions<MemoryDistributedCacheOptions>>();
			cacheOptions.Setup(m => m.Value)
				.Returns(new MemoryDistributedCacheOptions());
			var cache = new MemoryDistributedCache(cacheOptions.Object);
			var options = Mock.Of<IOptions<DistributedCacheEntryOptions>>();
			return new DistributedCacheStorage(cache, withOptions ? options : null);
		}
	}
}