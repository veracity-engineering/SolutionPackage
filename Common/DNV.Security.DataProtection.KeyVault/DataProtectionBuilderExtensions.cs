using Azure.Core;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace DNV.Security.DataProtection.KeyVault
{
	public static class DataProtectionBuilderExtensions
	{
		public static IDataProtectionBuilder PersistAndProtectKeysToAzureKeyVault(
			this IDataProtectionBuilder builder,
			Uri vaultUri,
			string keyName,
			string secretName,
			TokenCredential tokenCredential
		)
		{
			return builder.PersistKeysToAzureKeyVault(vaultUri, secretName, tokenCredential)
				.ProtectKeysWithAzureKeyVault(new Uri(vaultUri, "keys/" + keyName), tokenCredential);
		}

		public static IDataProtectionBuilder PersistKeysToAzureKeyVault(
			this IDataProtectionBuilder builder,
			Uri vaultUri,
			string secretName,
			TokenCredential tokenCredential
		)
		{
			builder.Services.AddAzureClients(b =>
			{
				b.UseCredential(tokenCredential);
				b.AddSecretClient(vaultUri)
					.WithName(nameof(AzureKeyVaultXmlRepository));
			});

			builder.Services.AddSingleton<IConfigureOptions<KeyManagementOptions>>(p =>
			{
				var loggerFactory = p.GetRequiredService<ILoggerFactory>();
				var secretClientFactory = p.GetRequiredService<IAzureClientFactory<SecretClient>>();
				var xmlRepository = new AzureKeyVaultXmlRepository(loggerFactory, secretClientFactory, vaultUri, secretName, tokenCredential);
				return new ConfigureOptions<KeyManagementOptions>(o => o.XmlRepository =
				xmlRepository);
			});
			return builder;
		}
	}
}