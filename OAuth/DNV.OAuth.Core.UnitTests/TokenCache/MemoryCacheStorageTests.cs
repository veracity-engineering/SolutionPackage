using Xunit;
using DNV.OAuth.Core.TokenCache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;

namespace DNV.OAuth.Core.TokenCache.UnitTests
{
	public class MemoryCacheStorageTests
	{
		[Fact]
		public async void MemoryCacheStorageTest()
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

		private static MemoryCacheStorage CreateSUT(bool withOptions)
		{
			var cache = new MemoryCache(new MemoryCacheOptions());
			var options = Mock.Of<IOptions<MemoryCacheEntryOptions>>();
			return new MemoryCacheStorage(cache, withOptions ? options : null);
		}
	}
}