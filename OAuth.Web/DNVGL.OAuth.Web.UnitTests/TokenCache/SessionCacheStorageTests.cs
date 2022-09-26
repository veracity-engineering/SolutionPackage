using Xunit;
using DNVGL.OAuth.Web.TokenCache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Net.NetworkInformation;

namespace DNVGL.OAuth.Web.TokenCache.UnitTests
{
	public class SessionCacheStorageTests
	{
		private static byte[]? BytesValue;

		[Fact()]
		public async void GetTest()
		{
			var (key1, value1) = ("item1", RandomNumberGenerator.GetBytes(10));
			var (key2, value2) = ("item2", RandomNumberGenerator.GetBytes(10));

			var sut = CreateSUT();
			sut.Set(key1, value2);
			sut.Get(key1);
			Assert.Equal(value2, BytesValue);

			await sut.SetAsync(key2, value1);
			await sut.GetAsync(key2);
			Assert.Equal(value1, BytesValue);

			sut.Remove(key1);
			sut.Get(key1);
			Assert.Null(BytesValue);

			await sut.RemoveAsync(key2);
			await sut.GetAsync(key2);
			Assert.Null(BytesValue);
		}

		private static SessionCacheStorage CreateSUT()
		{
			var storage = new Dictionary<string, byte[]>();
			var session = new Mock<ISession>();

			byte[]? value;
			session.Setup(m => m.TryGetValue(It.IsAny<string>(), out value))
				.Returns<string, byte[]?>((k,v) =>  
				{
					var isExists = storage.ContainsKey(k);
					BytesValue = isExists ? storage[k] : null;
					return isExists;
				});
			session.Setup(m => m.Remove(It.IsAny<string>()))
				.Callback<string>(key => storage.Remove(key));
			session.Setup(m => m.Set(It.IsAny<string>(), It.IsAny<byte[]>()))
				.Callback<string, byte[]>((key, value) => storage[key] = value);

			var accessor = new Mock<IHttpContextAccessor>();
			accessor.Setup(m => m.HttpContext)
				.Returns(new DefaultHttpContext { Session = session.Object });
			return new SessionCacheStorage(accessor.Object);
		}
	}
}