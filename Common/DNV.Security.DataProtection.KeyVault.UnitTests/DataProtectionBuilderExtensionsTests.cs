using Azure.Core;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace DNV.Security.DataProtection.KeyVault.UnitTests
{
	public class DataProtectionBuilderExtensionsTests
	{
		[Fact()]
		public void PersistAndProtectKeysToAzureKeyVaultTest()
		{
			var vaultUri = new Uri("https://example.com");
			var secretName = It.IsAny<string>();
			var keyName = It.IsAny<string>();
			var tokenCredential = Mock.Of<TokenCredential>();

			var services = new ServiceCollection();
			services.AddAzureClients(b =>
			{
				b.UseCredential(tokenCredential);
				b.AddSecretClient(vaultUri)
					.WithName(nameof(AzureKeyVaultXmlRepository));
			});

			services.AddDataProtection()
				.PersistAndProtectKeysToAzureKeyVault(vaultUri, secretName, "keys/" + keyName, tokenCredential);
			var serviceProvider = services.BuildServiceProvider();

			var keyManagementOptions = serviceProvider.GetRequiredService<IOptions<KeyManagementOptions>>().Value;
			Assert.NotNull(keyManagementOptions);
			Assert.IsType<AzureKeyVaultXmlRepository>(keyManagementOptions.XmlRepository);
		}
	}
}