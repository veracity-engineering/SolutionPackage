using DNV.OAuth.Abstractions;
using DNV.OAuth.Core.Exceptions;
using Moq;
using Xunit;

namespace DNV.OAuth.Core.UnitTests
{
	public class MsalClientAppFactoryTests
	{
		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public void MsalClientAppFactoryTest(bool withTokenCacheProvider)
		{
			var sut = CreateSUT(withTokenCacheProvider);

			IClientApp createForClient() => sut.CreateForClient(default);
			Assert.Throws<MissingScopeException>(createForClient);

			var clientApp = sut.CreateForClient(Guid.NewGuid().ToString());
			Assert.NotNull(clientApp);

			IClientApp createForUser() => sut.CreateForUser(null);
			Assert.Throws<MissingScopeException>(createForUser);

			clientApp = sut.CreateForUser(Guid.NewGuid().ToString());
			Assert.NotNull(clientApp);

			MsalClientAppFactory newMsalClientAppFactory() => new(default);
			Assert.Throws<ArgumentNullException>(newMsalClientAppFactory);
		}

		private static MsalClientAppFactory CreateSUT(bool withTokenCacheProvider)
		{
			var options = new VeracityClientOptions
			{
				ClientId = Guid.NewGuid().ToString(),
				ClientSecret = Guid.NewGuid().ToString()
			};
			var tokenCacheProvider = withTokenCacheProvider ? Mock.Of<ITokenCacheProvider>() : null;
			return new MsalClientAppFactory(options, tokenCacheProvider);
		}
	}
}