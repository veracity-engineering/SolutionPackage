using Azure;
using Azure.Core;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Linq;
using Xunit;

namespace DNV.Security.DataProtection.KeyVault.UnitTests
{
	public class AzureKeyVaultXmlRepositoryTests
	{
		[Fact]
		public void AzureKeyVaultXmlRepositoryTest()
		{
			var sut = CreateSUT("incorrect-name");
			var elements = sut.GetAllElements();
			Assert.Empty(elements);

			sut = CreateSUT("correct-name");
			elements = sut.GetAllElements();
			Assert.Equal(2, elements.Count);

			var newElement = new XElement(
				"key",
				new XAttribute("id", Guid.NewGuid().ToString("d")),
				new XElement("creationDate", DateTime.Now.ToString("u"))
			);
			sut.StoreElement(newElement, "Element One");
			elements = sut.GetAllElements();
			Assert.Equal(3, elements.Count);
		}

		private static AzureKeyVaultXmlRepository CreateSUT(string secretName)
		{
			var store = new NameValueCollection();
			var xml = $@"
<keys>
	<revocation>
	  <revocationDate>{DateTime.Now.AddDays(-10):u}</revocationDate>
	</revocation>
	<key id=""{Guid.NewGuid():d}"">
	  <creationDate>{DateTime.Now.AddDays(-118):u}</creationDate>
	</key>
	<key id=""{Guid.NewGuid():d}"">
	  <creationDate>{DateTime.Now.AddDays(-3):u}</creationDate>
	</key>
	<foo />
</keys>
";
			var vaultUri = "https://somekeyvault.vault.azure.net";
			store.Add("correct-name", Convert.ToBase64String(Encoding.UTF8.GetBytes(xml)));

			var secretClient = new Mock<SecretClient>();
			secretClient.Setup(m => m.GetSecret(It.IsAny<string>(), It.IsAny<string>(), default))
				.Returns<string, string, CancellationToken>((k, _, _) => MockResponse(k, store[k]));
			secretClient.Setup(m => m.SetSecret(It.IsAny<string>(), It.IsAny<string>(), default))
				.Callback<string, string, CancellationToken>((k, v, _) => store[k] = v);

			var secretClientFactory = new Mock<IAzureClientFactory<SecretClient>>();
			secretClientFactory.Setup(m => m.CreateClient(It.IsAny<string>()))
				.Returns(secretClient.Object);

			var tokenCredential = Mock.Of<TokenCredential>();

			return new AzureKeyVaultXmlRepository(NullLoggerFactory.Instance, secretClientFactory.Object, new Uri(vaultUri), secretName, tokenCredential);
		}

		private static Azure.Response<KeyVaultSecret> MockResponse(string name, string? value)
		{
			if (value == null) throw new RequestFailedException(404, "cannot find value");

			var keyVaultSerect = new KeyVaultSecret(name, value);
			var secretResponse = Azure.Response.FromValue(keyVaultSerect, Mock.Of<Azure.Response>());
			return secretResponse;
		}
	}
}