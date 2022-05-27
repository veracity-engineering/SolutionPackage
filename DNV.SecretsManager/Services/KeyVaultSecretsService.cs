using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Management.KeyVault;
using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Azure.Services.AppAuthentication;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Rest;
using System;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Rest.Azure;

namespace DNV.SecretsManager.Services
{
	public class KeyVaultSecretsService : SecretsService
	{
		private string _azureSubscriptionId;

		public KeyVaultSecretsService()
		{
		}

		public async Task<IEnumerable<KeyValuePair<string, string>>> GetSubscriptions()
		{
			var credentials = await GetManagementCredentials();
			var subscriptionClient = new SubscriptionClient(credentials);
			var subscriptions = await subscriptionClient.Subscriptions.ListAsync();
			return subscriptions.Select(s => new KeyValuePair<string, string>(s.DisplayName, s.SubscriptionId));
		}

		public void SetSubscriptionId(string value)
		{
			_azureSubscriptionId = value;
		}

		public override async Task<IEnumerable<KeyValuePair<string, string>>> GetSources()
		{
			var credentials = await GetManagementCredentials();
			var client = new KeyVaultManagementClient(credentials)
			{
				SubscriptionId = _azureSubscriptionId
			};
			var result = await client.Vaults.ListBySubscriptionAsync();
			var keyvaults = result.ToDictionary(v => v.Name, v => v.Properties.VaultUri);
			while (!string.IsNullOrEmpty(result.NextPageLink))
			{
				result = await client.Vaults.ListBySubscriptionNextAsync(result.NextPageLink);
				foreach (var keyvault in result)
				{
					keyvaults.Add(keyvault.Name, keyvault.Properties.VaultUri);
				}
			}
			return keyvaults.OrderBy(v => v.Key);
		}

		public override async Task<string> GetSecretsAsJson(string source) =>
			ToJson(await GetSecretsAsDictionary(source));

		public override Task SetSecretsFromJson(string source, string json) =>
			SetSecretsFromDictionary(source, FromJson(json));

		public override async Task<Dictionary<string, string>> GetSecretsAsDictionary(string source)
		{
			var keyVaultClient = new KeyVaultClient(await GetKeyVaultCredentials());
			var secretsDict = new Dictionary<string, string>();
			var secrets = await keyVaultClient.GetSecretsAsync(source);
			foreach (var secret in secrets)
			{
				var key = secret.Identifier.Name;
				var value = (await keyVaultClient.GetSecretAsync(secret.Identifier.Identifier)).Value;
				secretsDict.Add(key, value);
			}

			while (!string.IsNullOrEmpty(secrets.NextPageLink))
			{
				secrets = await keyVaultClient.GetSecretsNextAsync(secrets.NextPageLink);
				var tasks = secrets.Select(s => keyVaultClient.GetSecretAsync(s.Identifier.Identifier));
				var results = await Task.WhenAll(tasks);
				foreach (var secretValue in results)
				{
					secretsDict.Add(secretValue.SecretIdentifier.Name, secretValue.Value);
				}
				/*
				foreach (var secret in secrets)
				{
					var key = secret.Identifier.Name;
					var value = (await keyVaultClient.GetSecretAsync(secret.Identifier.Identifier)).Value;
					secretsDict.Add(key, value);
				}
				*/
			}
			return secretsDict;
		}

		public override async Task SetSecretsFromDictionary(string source, Dictionary<string, string> secrets)
		{
			var keyVaultClient = new KeyVaultClient(await GetKeyVaultCredentials());
			//var tasks = secrets.Select(s => keyVaultClient.SetSecretAsync(vaultBaseUrl, s.Key, secrets[s.Key], contentType: "text/plain"));
			//await Task.WhenAll(tasks);
			foreach (var secret in secrets)
			{
				Console.WriteLine($"Updating secret: '{secret.Key}'");
				await keyVaultClient.SetSecretAsync(source, secret.Key, secrets[secret.Key], contentType: "text/plain");
			}
		}

		public override async Task<int> ClearSecrets(string source)
		{
			var keyVaultClient = new KeyVaultClient(await GetKeyVaultCredentials());
			var secretKeys = new List<string>();
			var secrets = await keyVaultClient.GetSecretsAsync(source);
			secretKeys.AddRange(await GetSecretKeys(keyVaultClient, secrets));
			while (!string.IsNullOrEmpty(secrets.NextPageLink))
			{
				secrets = await keyVaultClient.GetSecretsNextAsync(secrets.NextPageLink);
				secretKeys.AddRange(await GetSecretKeys(keyVaultClient, secrets));
			}

			var deletedCount = 0;
			foreach (var secretKey in secretKeys)
			{
				Console.WriteLine($"Deleting secret: '{secretKey}'");
				await keyVaultClient.DeleteSecretAsync(source, secretKey);
				deletedCount++;
			}
			return deletedCount;
		}

		private static async Task<IEnumerable<string>> GetSecretKeys(KeyVaultClient keyVaultClient, IPage<SecretItem> secrets)
		{
			var tasks = secrets.Select(s => keyVaultClient.GetSecretAsync(s.Identifier.Identifier));
			var results = await Task.WhenAll(tasks);
			return results.Select(r => r.SecretIdentifier.Name);
		}

		public virtual async Task<ServiceClientCredentials> GetManagementCredentials()
		{
			var token = await new AzureServiceTokenProvider().GetAccessTokenAsync("https://management.azure.com/").ConfigureAwait(false);
			return new TokenCredentials(token);
		}

		public virtual Task<ServiceClientCredentials> GetKeyVaultCredentials()
		{
			var credentials = new KeyVaultCredential(new KeyVaultClient.AuthenticationCallback(new AzureServiceTokenProvider().KeyVaultTokenCallback));
			return Task.FromResult<ServiceClientCredentials>(credentials);
		}
	}
}
