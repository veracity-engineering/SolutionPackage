using Azure.Core;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
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
			builder.Services.AddSingleton(p =>
			{
				return new ConfigureOptions<KeyManagementOptions>(o =>
				{
					var loggerFactory = p.GetService<ILoggerFactory>() ?? NullLoggerFactory.Instance;
					o.XmlRepository = new AzureKeyVaultXmlRepository(vaultUri, secretName, tokenCredential);
				});
			});
			return builder;
		}
	}
}