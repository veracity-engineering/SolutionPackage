using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace DNV.Security.DataProtection.UnitTests
{
	public class DataProtectionBuilderExtensionsTests
	{
		[Fact]
		public void ProtectKeysWithPasskeyTest()
		{
			var passKey = Guid.NewGuid().ToString();
			var services = new ServiceCollection();
			services.AddDataProtection()
				.ProtectKeysWithPasskey(passKey);
			var serviceProvider = services.BuildServiceProvider();

			var passkeyXmlCipher = serviceProvider.GetService<PasskeyXmlCipher>();
			Assert.NotNull(passkeyXmlCipher);

			var passkeyXmlCipherOptions = serviceProvider.GetService<IOptions<PasskeyXmlCipherOptions>>()?.Value;
			Assert.NotNull(passkeyXmlCipherOptions);
			Assert.Equal(passKey, passkeyXmlCipherOptions.Passkey);

			var keyManagementOptions = serviceProvider.GetService<IOptions<KeyManagementOptions>>()?.Value;
			Assert.NotNull(keyManagementOptions);
			Assert.IsType<PasskeyXmlCipher>(keyManagementOptions.XmlEncryptor);
		}
	}
}