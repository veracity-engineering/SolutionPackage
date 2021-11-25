using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Management.KeyVault;
using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Azure.Services.AppAuthentication;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Rest;
using System;

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
			try
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
			catch (Exception)
			{
				throw;
			}
		}

		public override async Task<string> GetSecretsAsJson(string vaultBaseUrl) =>
			ToJson(await GetSecretsAsDictionary(vaultBaseUrl));

		public override Task SetSecretsFromJson(string vaultBaseUrl, string json) =>
			SetSecretsFromDictionary(vaultBaseUrl, FromJson(json));

		public override async Task<Dictionary<string, string>> GetSecretsAsDictionary(string vaultBaseUrl)
		{
			var keyVaultClient = new KeyVaultClient(await GetKeyVaultCredentials());
			var secretsDict = new Dictionary<string, string>();
			var secrets = await keyVaultClient.GetSecretsAsync(vaultBaseUrl);
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

		public override async Task SetSecretsFromDictionary(string vaultBaseUrl, Dictionary<string, string> secrets)
		{
			var keyVaultClient = new KeyVaultClient(await GetKeyVaultCredentials());
			//var tasks = secrets.Select(s => keyVaultClient.SetSecretAsync(vaultBaseUrl, s.Key, secrets[s.Key], contentType: "text/plain"));
			//await Task.WhenAll(tasks);
			foreach (var secret in secrets)
			{
				Console.WriteLine($"Updating secret: '{secret.Key}'");
				await keyVaultClient.SetSecretAsync(vaultBaseUrl, secret.Key, secrets[secret.Key], contentType: "text/plain");
			}
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
