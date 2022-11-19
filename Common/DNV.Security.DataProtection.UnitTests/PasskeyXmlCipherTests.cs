using Microsoft.Extensions.Options;
using Moq;
using System.Xml.Linq;
using Xunit;

namespace DNV.Security.DataProtection.UnitTests
{
	public class PasskeyXmlCipherTests
	{
		[Fact()]
		public void PasskeyXmlCipherTest()
		{
			var passKey = Guid.NewGuid().ToString();
			var options = new Mock<IOptions<PasskeyXmlCipherOptions>>();
			options.Setup(m => m.Value)
				.Returns(new PasskeyXmlCipherOptions { Passkey = passKey });

			var sut = new PasskeyXmlCipher(options.Object);

			var plainElement = new XElement("root", "value");
			var encryptedElement = sut.Encrypt(plainElement)?.EncryptedElement;
			Assert.NotNull(encryptedElement);
			Assert.NotEqual(plainElement.ToString(), encryptedElement.ToString());

			var decryptedElement = sut.Decrypt(encryptedElement);
			Assert.NotNull(decryptedElement);
			Assert.Equal(plainElement.ToString(), decryptedElement.ToString());

			PasskeyXmlCipher action1() => new(Mock.Of<IOptions<PasskeyXmlCipherOptions>>());
			Assert.Throws<ArgumentNullException>(action1);

			PasskeyXmlCipher action2() => new(new PasskeyXmlCipherOptions());
			Assert.Throws<ArgumentNullException>(action2);
		}
	}
}